using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TaskPhases
{
    public sealed class ArithmeticPhase : TaskPhase
    {
        public float TimeLeft { get; private set; }
        public float Duration
        {
            get => _duration; 
            private set => _duration = value; 
        }
        [SerializeField]
        private float _duration;

        protected override void OnEnable()
        {
            TimeLeft = Duration;
            base.OnEnable();
        }

        protected override int GetTrialCount() => nTrials = (int)Duration * 10;

        protected override IEnumerator RunNewTrial()
        {
            OrderTrialStages();

            // For each trial stage, check if active. 
            // If not active, set to active and do nothing until it's not again
            for (int i = 0; i < trialStagesInOrder.Length; i++)
            {
                if (!trialStagesInOrder[i].activeSelf)
                {
                    trialStagesInOrder[i].SetActive(true);
                    while (trialStagesInOrder[i].activeSelf && TimeLeft > 0f)
                    {
                        TimeLeft -= Time.deltaTime;
                        yield return null;
                    }
                }
            }

            if (TrialIdx < nTrials - 1 && TimeLeft > 0f)
            {
                TrialIdx++;
                StartCoroutine(RunNewTrial());
            }
            else
            {
                StartCoroutine(WaitToEnd());
                yield break;
            }

        }

        protected override void OrderTrialStages() => trialStagesInOrder = TrialStages.ToArray();
        protected override void OnDisable()
        {
            if (Global.SessionInfo.IsPractice)
            {
                SceneManager.LoadScene("TestPhase Instructions");
            }
            else
            {
                base.OnDisable();
            }
        }
    }
}