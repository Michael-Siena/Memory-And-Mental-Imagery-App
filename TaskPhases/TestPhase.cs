using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using CustomExtensions;
using CustomDataTypes;

namespace TaskPhases.Test
{
    public sealed class TestPhase : TaskPhase
    {
        public static List<TestTrialData> BlockTrialData { get; private set; }

        protected override void Awake() => base.Awake();
        protected override void OnEnable()
        {
            BlockTrialData = (Global.SessionInfo.IsPractice)
                ? ReadTrialData($"{Global.DataPath}/Practice/Practice_Test")
                : ReadTrialData($"{Global.DataPath}/MainTask/sub{Global.SessionInfo.SID}/sub{Global.SessionInfo.SID}_block{Global.SessionInfo.Block + 1}_test");

            base.OnEnable();
        }

        protected override int GetTrialCount() => BlockTrialData.Count;

        private List<TestTrialData> ReadTrialData(string path)
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

            var blockTrialData = new List<TestTrialData>(lines.Count);
            foreach (string line in lines)
            {
                string[] values = line.Split(',');
                var trialData = new TestTrialData()
                {
                    TargetName = values[0],
                    StudiedHueRotation = float.Parse(values[1]),
                    ShownHueRotation = float.Parse(values[2]),
                    StudiedTargetPosition = new Vector3(float.Parse(values[3]), float.Parse(values[4]), float.Parse(values[5])),
                    MemoryTestOrder = (MemoryTestOrderType)int.Parse(values[6]),
                    StudiedPerspective = (PerspectiveType)int.Parse(values[7]),
                    TestedPerspective = (PerspectiveType)int.Parse(values[8]),
                    SwitchStatus = (SwitchStatusType)int.Parse(values[9])
                };
                blockTrialData.Add(trialData);
            }
            return blockTrialData;
        }

        protected override IEnumerator RunNewTrial()
        {
            yield return StartCoroutine(base.RunNewTrial());
        }

        protected override void OrderTrialStages()
        {
            int colorVividnessIndex = TrialStages.FindIndex(go => go.name == "ColorVividnessRating");
            int objectMemoryIndex = TrialStages.FindIndex(go => go.name == "ObjectMemoryTest");
            int locationVividnessIndex = TrialStages.FindIndex(go => go.name == "LocationVividnessRating");
            int spatialMemoryIndex = TrialStages.FindIndex(go => go.name == "SpatialMemoryTest");

            trialStagesInOrder = new GameObject[TrialStages.Count];

            switch (BlockTrialData[TrialIdx].MemoryTestOrder)
            {
                case MemoryTestOrderType.ObjectThenSpatial:
                    trialStagesInOrder = TrialStages.ToArray();
                    break;
                case MemoryTestOrderType.SpatialThenObject:
                    trialStagesInOrder = TrialStages.ToArray().SwapElements(locationVividnessIndex, colorVividnessIndex);
                    trialStagesInOrder = trialStagesInOrder.ToArray().SwapElements(spatialMemoryIndex, objectMemoryIndex);
                    break;
            }
        }

        protected override string GetEndOfPhasePrompt()
        {
            return (Global.SessionInfo.IsPractice)
                ? "End of practice test phase.\n\nYou may take a short break now or press 'space' to start the main task."
                : $"End of test phase.\n\nYou may take a short break now or press 'space' to {((Global.SessionInfo.Block < Global.NBlocks - 1) ? "continue" : "finish the main task")}.";
        }

        protected override void OnDisable()
        {
            if (Global.SessionInfo.Block < Global.NBlocks - 1)
            {
                if (Global.SessionInfo.IsPractice)
                {
                    Global.SessionInfo.IsPractice = false;
                    SceneManager.LoadScene("EndOfPractice");
                }
                else
                {
                    Global.SessionInfo.Block++;
                    base.OnDisable();
                }
            }
            else
            {
                switch (Global.SessionInfo.TaskOrder)
                {
                    case TaskOrderType.MemoryTaskThenQuestionnaires:
                        SceneManager.LoadScene("QuestionnaireOverview Instructions");
                        break;
                    case TaskOrderType.QuestionnairesThenMemoryTask:
                        SceneManager.LoadScene(SceneManager.sceneCount - 1); // load final scene
                        break;
                }
            }
        }
    }
}