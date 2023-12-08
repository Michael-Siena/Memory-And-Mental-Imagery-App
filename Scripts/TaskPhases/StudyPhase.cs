using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CustomDataTypes;

namespace TaskPhases.Study
{
    public sealed class StudyPhase : TaskPhase
    {
        // Target stuff
        public GameObject Target
        {
            get => _target;
            private set => _target = value;
        }
        [SerializeField]
        private GameObject _target;
        private string spritePath;

        // Participant stuff
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

        public static List<StudyTrialData> BlockTrialData { get; private set; }
        protected override void Awake() => base.Awake();

        protected override void OnEnable()
        {
            spritePath = Global.SpritePath;
            if (Global.SessionInfo.IsPractice)
            {
                BlockTrialData = ReadTrialData($"{Global.DataPath}/Practice/Practice_Study");
                spritePath += "/Practice";
            }
            else
            {
                BlockTrialData = ReadTrialData($"{Global.DataPath}/MainTask/sub{Global.SessionInfo.SID}/sub{Global.SessionInfo.SID}_block{Global.SessionInfo.Block + 1}_study");
                spritePath += "/MainTask";
            }

            base.OnEnable();
        }

        protected override int GetTrialCount() => BlockTrialData.Count;

        private List<StudyTrialData> ReadTrialData(string path)
        {
            // Split raw TextAsset by new line, skip first line (the headers),
            // take non-empty lines and assign to array. 
            // Parse each line into a trial data object and add to block trial data collection
            TextAsset rawData = Resources.Load<TextAsset>(path);
            List<string> lines = rawData.text
                .Split('\n')
                .Skip(1)
                .Where(ln => ln != string.Empty)
                .ToList();

            var blockTrialData = new List<StudyTrialData>(lines.Count);
            foreach (string line in lines)
            {
                string[] values = line.Split(',');
                var trialData = new StudyTrialData()
                {
                    TargetName = values[0],
                    TargetHueRotation = float.Parse(values[1]),
                    TargetPosition = new Vector3(float.Parse(values[2]), float.Parse(values[3]), float.Parse(values[4])),
                    Perspective = (PerspectiveType)int.Parse(values[5])
                };
                blockTrialData.Add(trialData);
            }

            return blockTrialData;
        }

        protected override IEnumerator RunNewTrial()
        {
            Target.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"{spritePath}/{BlockTrialData[TrialIdx].TargetName}");

            yield return StartCoroutine(base.RunNewTrial());
        }

        protected override void OrderTrialStages() => trialStagesInOrder = TrialStages.ToArray();

        protected override void OnDisable()
        {
            if (Global.SessionInfo.IsPractice)
            {
                SceneManager.LoadScene("ArithmeticPhase Instructions");
            }
            else
            {
                base.OnDisable();
            }
        }
    }
}