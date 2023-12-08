using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ColorSpaces;
using CustomExtensions;
using TaskPhases.Study;
using CustomDataTypes;

namespace TrialStages
{
    public sealed class StudyMemoryArena : TrialStage, IRotatesHue
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
        // Participants
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
        // Target
        public GameObject Target
        {
            get => _target;
            private set => _target = value;
        }
        [SerializeField]
        private GameObject _target;
        private string spritePath;

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

        public Texture2D OriginalColorTexture { get; private set; }
        public Texture2D CurrentTexture { get; private set; }
        public byte[] RawOriginalPixelData { get; private set; }
        public Color32[] OriginalRGBAColors { get; private set; }
        public LABColor[] OriginalLABColors { get; private set; }

        // Landmarks
        public List<GameObject> Landmarks
        {
            get => _landmarks;
            private set => _landmarks = value;
        }
        [SerializeField]
        private List<GameObject> _landmarks;
        private OrientationCueData orientationCueData;
        private string orientationCuesPath;

        private StudyTrialData trialData;

        protected override void OnEnable()
        {
            trialData = StudyPhase.BlockTrialData[StudyPhase.TrialIdx];

            if (Global.SessionInfo.IsPractice)
            {
                orientationCueData = ReadOrientationCues($"{Global.DataPath}/Practice/Practice_OrientationCues");
                orientationCuesPath = "Sprites/Landmarks/Practice";
                spritePath = $"{Global.SpritePath}/Practice";
            }
            else
            {
                orientationCueData = ReadOrientationCues($"{Global.DataPath}/MainTask/sub{Global.SessionInfo.SID}/sub{Global.SessionInfo.SID}_block{Global.SessionInfo.Block + 1}_orientationCues");
                orientationCuesPath = "Sprites/Landmarks/MainTask";
                spritePath = $"{Global.SpritePath}/MainTask";
            }

            // set up arena objects
            IEnumerator<string> ocdEnum = orientationCueData.GetEnumerator();
            for (int i = 0; i < Landmarks.Count; i++)
            {
                ocdEnum.MoveNext();
                Landmarks[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"{orientationCuesPath}/{ocdEnum.Current}");
            }
            ParticipantBody.SetActive(false);
            CenterParticipant();
            Target.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"{spritePath}/{trialData.TargetName}");
            LoadTarget(trialData.TargetName, trialData.TargetPosition);

            // Cache original texture data in Color32 and LAB colorspaces
            CurrentTexture = Target.GetComponent<SpriteRenderer>().sprite.texture;
            OriginalColorTexture = CurrentTexture.CopyTexture();
            RawOriginalPixelData = OriginalColorTexture.GetRawTextureData();
            OriginalRGBAColors = OriginalColorTexture.GetPixels32();
            OriginalLABColors = OriginalRGBAColors.ToLABPixels32();

            // rotate hue
            CurrentTexture.SetPixelData(RawOriginalPixelData, 0, 0);
            CurrentTexture.RotatePixelHues32(OriginalRGBAColors, OriginalLABColors, trialData.TargetHueRotation);

            // set perspective
            MainCamera.SetActive(false);
            switch (trialData.Perspective)
            {
                case PerspectiveType.FirstPerson:
                    FirstPersonCamera.SetActive(true);
                    ThirdPersonCamera.SetActive(false);

                    FaceTarget(Participant, Target);

                    controlsText.text = fppControlsText;
                    break;
                case PerspectiveType.ThirdPerson:
                    FirstPersonCamera.SetActive(false);
                    ThirdPersonCamera.SetActive(true);

                    Participant.transform.rotation = Quaternion.identity; // lock rotation

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

        private void CenterParticipant() => Participant.transform.position = Vector3.zero;

        private void LoadTarget(string name, Vector3 pos)
        {
            Target.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"{spritePath}/{name}");
            Target.transform.position = pos;
        }

        private void FaceTarget(GameObject obj, GameObject target) => obj.transform.LookAt(new Vector3(target.transform.position.x, 0f, target.transform.position.z));

        protected override void OnDisable()
        {
            base.OnDisable();

            CenterParticipant();
            MainCamera.SetActive(true);
        }
    }
}