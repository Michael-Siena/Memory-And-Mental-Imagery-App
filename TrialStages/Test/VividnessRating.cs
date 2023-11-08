using CustomDataTypes;
using UnityEngine;
using UnityEngine.UI;
using FirebaseWebGL;
using TaskPhases.Test;

namespace TrialStages.Test
{
    public sealed class VividnessRating : TrialStage, ICollectsData<float>
    {
        public GameObject Panel
        {
            get => _panel;
            private set => _panel = value;
        }
        [SerializeField]
        private GameObject _panel;
        public string prompt;
        private Slider slider;
        private readonly float sliderDefaultValue = 50f;
        public float SliderSpeed;

        private TestTrialData trialData;

        private void Awake()
        {
            slider = Panel.transform.Find("Slider").GetComponent<Slider>();
        }

        protected override void OnEnable()
        {
            trialData = TestPhase.BlockTrialData[TestPhase.TrialIdx];

            Panel.SetActive(true);
            Panel.transform.Find("PromptText").gameObject.GetComponent<Text>().text = prompt;

            base.OnEnable();
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKey(KeyCode.A))
            {
                slider.value -= 1f * Time.deltaTime * SliderSpeed;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                slider.value += 1f * Time.deltaTime * SliderSpeed;
            }
            else if (Input.GetKeyUp(KeyCode.Space) || TimeLeft <= 0f)
            {
                var VividnessData = new ResponseData<float>(slider.value, Duration - TimeLeft);
                Debug.Log($"{this.gameObject.name}: {slider.value}");

                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    CollectData(VividnessData);
                    Debug.Log(this.gameObject.name);
                }

                this.gameObject.SetActive(false);
            }
        }

        protected override void OnDisable()
        {
            Input.ResetInputAxes();

            Panel.SetActive(false);
            slider.value = sliderDefaultValue;

            base.OnDisable();
        }

        public void CollectData(ResponseData<float> responseData)
        {
            string fullPath = (Global.SessionInfo.IsPractice)
                ? $"{Global.FirebasePath}/SID{Global.SessionInfo.SID}/Practice/TestPhase/Trial{TestPhase.TrialIdx}/{this.gameObject.name}"
                : $"{Global.FirebasePath}/SID{Global.SessionInfo.SID}/Block{Global.SessionInfo.Block}/TestPhase/Trial{TestPhase.TrialIdx}/{this.gameObject.name}";
            FirebaseDatabase.PostJSON(fullPath, responseData.ToString(), this.gameObject.name, "", "");
        }
    }
}