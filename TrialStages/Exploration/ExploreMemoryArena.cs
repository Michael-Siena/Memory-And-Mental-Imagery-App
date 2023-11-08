using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TaskPhases.Exploration;
using TrialStages;
using CustomDataTypes;

namespace TaskPhases.Arithmetic
{
    public sealed class ExploreMemoryArena : TrialStage
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
            "\n'Mouse right' = turn right";
        private string tppControlsText = "'A' = move left" +
            "\n'D' = move right" +
            "\n'W' = move up" +
            "\n'S' = move down" +
            "\n'Mouse up' = zoom in" +
            "\n'Mouse down' = zoom out";

        private OrientationCueData orientationCueData;
        private string orientationCuesPath;

        protected override void OnEnable()
        {
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

            // load orientation cue sprites
            IEnumerator<string> ocdEnum = orientationCueData.GetEnumerator();
            for (int i = 0; i < Landmarks.Count; i++)
            {
                ocdEnum.MoveNext();
                Landmarks[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"{orientationCuesPath}/{ocdEnum.Current}");
            }

            // set perspective
            MainCamera.SetActive(false);
            switch (orientationCueData.InitialPerspective)
            {
                case 1:
                    if (ExplorationPhase.TrialIdx == 0)
                    {
                        FirstPersonCamera.SetActive(true);
                        ThirdPersonCamera.SetActive(false);

                        controlsText.text = fppControlsText;
                    }
                    else if (ExplorationPhase.TrialIdx == 1)
                    {
                        FirstPersonCamera.SetActive(false);
                        ThirdPersonCamera.SetActive(true);

                        controlsText.text = tppControlsText;
                    }
                    break;
                case 2:
                    if (ExplorationPhase.TrialIdx == 0)
                    {
                        FirstPersonCamera.SetActive(false);
                        ThirdPersonCamera.SetActive(true);

                        controlsText.text = tppControlsText;
                    }
                    else if (ExplorationPhase.TrialIdx == 1)
                    {
                        FirstPersonCamera.SetActive(true);
                        ThirdPersonCamera.SetActive(false);

                        controlsText.text = fppControlsText;
                    }
                    break;
            }

            ParticipantBody.SetActive(false);
            CenterParticipant();

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
                WestName = values[3].Trim(),
                InitialPerspective = int.Parse(values[4].Trim())
            };
        }

        private void CenterParticipant()
        {
            Participant.transform.SetPositionAndRotation(new Vector3(0f, 0.55f, 0f), // center in memory arena, slightly above ground to avoid texture clipping
                                                         Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)); // face random direction
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            MainCamera.SetActive(true);

            CenterParticipant();
        }
    }
}