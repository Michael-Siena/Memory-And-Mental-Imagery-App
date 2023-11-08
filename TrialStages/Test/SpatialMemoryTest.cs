using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomDataTypes;
using TaskPhases.Test;
using FirebaseWebGL;

namespace TrialStages.Test
{
    public sealed class SpatialMemoryTest : TrialStage, ICollectsData<(Vector3 responseLocation, float euclideanDistance)>
    {
        // Cameras
        public GameObject MainCamera
        {
            get => _mainCamera;
            private set => _mainCamera = value;
        }
        [SerializeField]
        private GameObject _mainCamera;
        public GameObject FirstPersonCamera
        {
            get => _fpCamera;
            private set => _fpCamera = value;
        }
        [SerializeField]
        private GameObject _fpCamera;
        public GameObject ThirdPersonCamera
        {
            get => _tpCamera;
            private set => _tpCamera = value;
        }
        [SerializeField]
        private GameObject _tpCamera;
        // Participant
        public GameObject Participant
        {
            get => _participant;
            private set => _participant = value;
        }
        [SerializeField]
        private GameObject _participant;
        public GameObject ParticipantBody
        {
            get => _participantBody;
            private set => _participantBody = value;
        }
        [SerializeField]
        private GameObject _participantBody;
        //Landmarks
        public List<GameObject> Landmarks
        {
            get => _landmarks;
            private set => _landmarks = value;
        }
        [SerializeField]
        private List<GameObject> _landmarks;

        [SerializeField]
        private Text controlsText;
        private string fppControlsText = "'A' = move left" +
            "\n'D' = move right" +
            "\n'W' = move forward" +
            "\n'S' = move backward" +
            "\n'Mouse left' = turn left" +
            "\n'Mouse right' = turn right" +
            "\n'Space' = submit response";
        private string tppControlsText = "'A' = move left" +
            "\n'D' = move right" +
            "\n'W' = move up" +
            "\n'S' = move down" +
            "\n'Mouse up' = zoom in" +
            "\n'Mouse down' = zoom out" +
            "\n'Space' = submit response";

        private OrientationCueData orientationCueData;
        private string orientationCuesPath;
        private TestTrialData trialData;

        protected override void OnEnable()
        {
            trialData = TestPhase.BlockTrialData[TestPhase.TrialIdx];

            if (Global.SessionInfo.IsPractice)
            {
                orientationCueData = ReadOrientationCues($"{Global.DataPath}/Practice/Practice_OrientationCues");
                orientationCuesPath = "Sprites/Landmarks/Practice";
            }
            else
            {
                orientationCueData = ReadOrientationCues($"{Global.DataPath}/MainTask/sub{Global.SessionInfo.SID}/sub{Global.SessionInfo.SID}_block{Global.SessionInfo.Block + 1}_orientationCues");
                orientationCuesPath = "Sprites/Landmarks/MainTask";
            }

            // set up arena objects
            IEnumerator<string> ocdEnum = orientationCueData.GetEnumerator();
            for (int i = 0; i < Landmarks.Count; i++)
            {
                ocdEnum.MoveNext();
                Landmarks[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"{orientationCuesPath}/{ocdEnum.Current}");
            }
            CenterParticipant();
            ParticipantBody.SetActive(true);

            // set perspective and display appropriate controls
            MainCamera.SetActive(false);
            switch (trialData.TestedPerspective)
            {
                case PerspectiveType.FirstPerson:
                    FirstPersonCamera.SetActive(true);
                    ThirdPersonCamera.SetActive(false);

                    controlsText.text = fppControlsText;
                    break;
                case PerspectiveType.ThirdPerson:
                    FirstPersonCamera.SetActive(false);
                    ThirdPersonCamera.SetActive(true);

                    controlsText.text = tppControlsText;
                    break;
            }

            base.OnEnable();
        }

        private OrientationCueData ReadOrientationCues(string path)
        {
            TextAsset rawData = Resources.Load<TextAsset>(path);
            List<string> lines = rawData.text
                .Split('\n')
                .Skip(1)
                .Where(ln => ln != string.Empty)
                .ToList();

            string[] values = lines[0].Split(',');

            return new OrientationCueData()
            {
                NorthName = values[0].Trim(),
                SouthName = values[1].Trim(),
                EastName = values[2].Trim(),
                WestName = values[3].Trim()
            };
        }

        private void CenterParticipant()
        {
            Participant.transform.SetPositionAndRotation(new Vector3(0f, 0.55f, 0f), // center in memory arena, slightly above ground to avoid texture clipping
                                                         Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)); // face random direction
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyUp(KeyCode.Space) || TimeLeft <= 0f)
            {
                Vector3 responseLocation = ParticipantBody.transform.position;
                var LocationData = new ResponseData<(Vector3, float)>((responseLocation, EuclideanDistance(responseLocation, trialData.StudiedTargetPosition)), Duration - TimeLeft);
                Debug.Log($"{this.gameObject.name}: {LocationData.ToString()}");

                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    CollectData(LocationData);
                }

                this.gameObject.SetActive(false);
            }
        }

        protected override void OnDisable()
        {
            Input.ResetInputAxes();

            base.OnDisable();

            CenterParticipant();
            MainCamera.SetActive(true);
        }

        // We don't factor in the y-axis because (1) it's irrelevant and
        // (2) there can be an offset depending on collision...
        private float EuclideanDistance(Vector3 p, Vector3 q)
        {
            float dX = p.x - q.x;
            float dZ = p.z - q.z;

            return Mathf.Sqrt((dX * dX) + (dZ * dZ));
        }

        public void CollectData(ResponseData<(Vector3, float)> responseData)
        {
            string fullPath = (Global.SessionInfo.IsPractice)
                ? $"{Global.FirebasePath}/SID{Global.SessionInfo.SID}/Practice/TestPhase/Trial{TestPhase.TrialIdx}/SpatialMemoryPerformance"
                : $"{Global.FirebasePath}/SID{Global.SessionInfo.SID}/Block{Global.SessionInfo.Block}/TestPhase/Trial{TestPhase.TrialIdx}/SpatialMemoryPerformance";
            FirebaseDatabase.PostJSON(fullPath, responseData.ToString(), this.gameObject.name, "", "");
        }
    }
}