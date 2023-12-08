using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Runtime.InteropServices;
using CustomDataTypes;
using FirebaseWebGL;

namespace UI.PanelLogic
{
    public class ConsentPanel : MonoBehaviour
    {
        public Button StartStudyButton;
        public Toggle[] CheckBoxes;

        private void Awake() => StartStudyButton.interactable = false;

        [DllImport("__Internal")]
        private static extern void OpenWindow(string url);

        public void OnCheckBoxToggle()
        {
            foreach (Toggle cb in CheckBoxes)
            {
                if (!cb.isOn)
                {
                    StartStudyButton.interactable = false;
                    return;
                }
            }

            StartStudyButton.interactable = true;
        }

        public void OnStartStudyButton()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                FirebaseDatabase.PostJSON($"{Global.FirebasePath}/SID{Global.SessionInfo.SID}/Started",
                    Global.SessionInfo.StartTime.ToString(),
                    this.gameObject.name,
                    "",
                    "");
            }

            if (Global.SessionInfo.TaskOrder == TaskOrderType.MemoryTaskThenQuestionnaires)
            {
                SceneManager.LoadScene(1); // go to task instructions
            }
            switch (Global.SessionInfo.TaskOrder)
            {
                case TaskOrderType.MemoryTaskThenQuestionnaires: 
                    SceneManager.LoadScene(1); // go to task instructions
                    break;
                case TaskOrderType.QuestionnairesThenMemoryTask:
                    switch (Global.SessionInfo.QuestionnaireOrder)
                    {
                        case QuestionnaireOrderType.OSIQthenSAMthenVVIQ: case QuestionnaireOrderType.OSIQthenVVIQthenSAM:
                            SceneManager.LoadScene(QuestionnaireType.OSIQ.ToString());
                            break;
                        case QuestionnaireOrderType.SAMthenOSIQthenVVIQ: case QuestionnaireOrderType.SAMthenVVIQthenOSIQ:
                            SceneManager.LoadScene(QuestionnaireType.SAM.ToString());
                            break;
                        case QuestionnaireOrderType.VVIQthenOSIQthenSAM: case QuestionnaireOrderType.VVIQthenSAMthenOSIQ:
                            SceneManager.LoadScene(QuestionnaireType.VVIQ.ToString());
                            break;
                    }
                    break;
            }
        }
    }
}