using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TaskPhases;
using CustomDataTypes;
using FirebaseWebGL;

namespace TrialStages.Arithmetic
{
    public sealed class ArithmeticTest : TrialStage, ICollectsData<int>
    {
        public GameObject Panel
        {
            get => _panel;
            private set => _panel = value;
        }
        [SerializeField]
        private GameObject _panel;

        private Text problemText; 
        private ArithmeticProblemData arithmeticProblem;
        public ResponseData<int> ResponseData { get; private set; }

        private Stack<string> responseLog;
        private string currResponse;
        private string sign;
        private int nDigitsInAnswer;

        private void Awake() => problemText = Panel.GetComponent<Text>();

        protected override void OnEnable()
        {
            base.OnEnable();

            Panel.SetActive(true);

            arithmeticProblem = new ArithmeticProblemData();
            nDigitsInAnswer = (arithmeticProblem.Answer < 0) 
                ? arithmeticProblem.Answer.ToString().Length - 1
                : arithmeticProblem.Answer.ToString().Length;
            sign = (arithmeticProblem.Answer < 0) ? "-" : "";
            currResponse = new string('#', nDigitsInAnswer);

            problemText.text = $"{arithmeticProblem} = {sign}{currResponse}";

            responseLog = new Stack<string>();
        }

        protected override void Update()
        {
            base.Update();

            if (int.TryParse(Input.inputString.ToLower(), out int n))
            {
                if (((n > 0 && n <= 9) && responseLog.Count == 0) // rules for first digit
                    || ((n >= 0 && n <= 9) && (responseLog.Count > 0 && responseLog.Count < nDigitsInAnswer)) // rules for subsequent digits
                    || (n == 0 && arithmeticProblem.Answer == 0 && responseLog.Count == 0)) // special case where answer is 0. 
                {
                    responseLog.Push(n.ToString());

                    currResponse = currResponse.Remove(responseLog.Count - 1, 1).Insert(responseLog.Count - 1, responseLog.Peek());

                    problemText.text = $"{arithmeticProblem} = {sign}{currResponse}";
                }
            }
            else if (Input.inputString == "\b")
            {
                if (responseLog.Count > 0)
                {
                    currResponse = currResponse.Remove(responseLog.Count - 1, 1).Insert(responseLog.Count - 1, "#");

                    problemText.text = $"{arithmeticProblem} = {sign}{currResponse}";

                    responseLog.Pop();
                }
            }
            else if (Input.inputString == " ")
            {
                if (responseLog.Count == currResponse.Length)
                {
                    ResponseData = new ResponseData<int>(int.Parse(sign + currResponse), Duration - TimeLeft);
                    if (Application.platform == RuntimePlatform.WebGLPlayer)
                    {
                        CollectData(ResponseData);
                    }

                    this.gameObject.SetActive(false);
                }
            }
        }

        public void CollectData(ResponseData<int> responseData)
        {
            string fullPath = (Global.SessionInfo.IsPractice)
                 ? $"{Global.FirebasePath}/SID{Global.SessionInfo.SID}/Practice/Arithmetic/Trial{ArithmeticPhase.TrialIdx}/Performance"
                 : $"{Global.FirebasePath}/SID{Global.SessionInfo.SID}/Block{Global.SessionInfo.Block}/Arithmetic/Trial{ArithmeticPhase.TrialIdx}/Performance";
            string fullData = $"{arithmeticProblem}, {arithmeticProblem.Answer}, {responseData.ToString()}";
            FirebaseDatabase.PostJSON(fullPath, fullData, this.gameObject.name, "", "");
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Panel.SetActive(false);
        }
    }
}