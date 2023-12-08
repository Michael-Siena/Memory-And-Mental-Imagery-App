using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace TaskPhases
{
    public abstract class TaskPhase : MonoBehaviour, IWaitable
    {
        protected string formattedPhaseName;
        // Panel stuff
        public GameObject StartPanel
        {
            get => _startPanel;
            protected set => _startPanel = value;
        }
        [SerializeField]
        protected GameObject _startPanel;

        public GameObject EndPanel
        {
            get => _endPanel;
            protected set => _endPanel = value;
        }
        [SerializeField]
        protected GameObject _endPanel;
        protected Text startPhasePanelText, endPhasePanelText;

        public List<GameObject> TrialStages;
        protected GameObject[] trialStagesInOrder;

        public static int TrialIdx { get; protected set; }
        protected int nTrials;

        public string NextSceneName 
        {
            get => _nextSceneName;
            protected set => _nextSceneName = value;
        }
        [SerializeField]
        protected string _nextSceneName;


        protected virtual void Awake()
        {
            formattedPhaseName = Regex.Replace(this.GetType().Name, "([a-z])([A-Z])", "$1 $2").ToLower();

            startPhasePanelText = _startPanel.GetComponent<Text>();
            endPhasePanelText = _endPanel.GetComponent<Text>();
        }

        protected virtual void OnEnable()
        {
            nTrials = GetTrialCount();
            TrialIdx = 0;

            StartCoroutine(WaitToStart());
        }
        protected abstract int GetTrialCount();

        public virtual IEnumerator WaitToStart()
        {
            // wait for participant input to start phase
            StartPanel.SetActive(true);
            startPhasePanelText.text = GetStartOfPhasePrompt();

            while (!Input.GetKeyUp(KeyCode.Space))
            {
                yield return null;
            }

            StartPanel.SetActive(false);
            StartCoroutine(RunNewTrial());
        }

        protected virtual string GetStartOfPhasePrompt()
        {
            return (Global.SessionInfo.IsPractice)
                ? $"Press 'space' to start practicing the {formattedPhaseName}."
                : $"Press 'space' to start the {formattedPhaseName} of block {Global.SessionInfo.Block + 1} / {Global.NBlocks}.";
        }

        // the basic logic for runnig a trial, which can be overridden in derived classes
        protected virtual IEnumerator RunNewTrial()
        {
            OrderTrialStages();

            // For each trial stage, check if active. 
            // If not active, set to active and do nothing until it's not again
            for (int i = 0; i < trialStagesInOrder.Length; i++)
            {
                if (!trialStagesInOrder[i].activeSelf)
                {
                    trialStagesInOrder[i].SetActive(true);
                    while (trialStagesInOrder[i].activeSelf)
                    {
                        yield return null;
                    }
                }
            }

            if (TrialIdx < nTrials - 1)
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

        protected abstract void OrderTrialStages();

        public virtual IEnumerator WaitToEnd()
        {
            foreach (GameObject trialStage in TrialStages)
            {
                if (trialStage.activeSelf)
                {
                    trialStage.SetActive(false);
                }
            }

            // wait for participant input to end phase
            EndPanel.SetActive(true);
            endPhasePanelText.text = GetEndOfPhasePrompt();

            Input.ResetInputAxes();
            while (!Input.GetKeyUp(KeyCode.Space))
            {
                yield return null;
            }

            EndPanel.SetActive(false);
            this.gameObject.SetActive(false);
        }

        protected virtual string GetEndOfPhasePrompt()
        {
            return (Global.SessionInfo.IsPractice)
                ? $"Press 'space' to finish practicing the {formattedPhaseName}."
                : $"End of {formattedPhaseName}. Press 'space' to continue.";
        }

        protected virtual void OnDisable() => SceneManager.LoadScene(NextSceneName);
    }
}