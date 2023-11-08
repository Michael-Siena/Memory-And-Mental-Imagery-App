using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CustomDataTypes;

namespace TaskPhases.Exploration
{
    public sealed class ExplorationPhase : TaskPhase
    {
        protected override int GetTrialCount() => 2;
        protected override void OrderTrialStages() => trialStagesInOrder = TrialStages.ToArray();

        protected override void OnDisable()
        {
            if (Global.SessionInfo.IsPractice)
            {
                SceneManager.LoadScene("StudyPhase Instructions");
            }
            else
            {
                base.OnDisable();
            }
        }
    }
}