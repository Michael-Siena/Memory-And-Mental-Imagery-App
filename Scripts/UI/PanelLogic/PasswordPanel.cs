using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FirebaseWebGL;
using CustomDataTypes;

namespace UI.PanelLogic
{
    public class PasswordPanel : MonoBehaviour
    {
        public Button UnlockButton, StartButton;
        public TMP_InputField PasswordInputField;

        public GameObject NextPanel;
        
        protected struct Counterbalancing
        {
            public int SID { get; set; }
            public TaskOrderType TaskOrder;
            public QuestionnaireOrderType QuestionnaireOrder;
        }
        private Counterbalancing counterbalancing;

        private Dictionary<string, Counterbalancing> counterbalancingList;
        private readonly string sessionInfoFilePath = "Data/Counterbalancing";

        private string jsonData;

        // Start is called before the first frame update
        private void Awake()
        {
            StartButton.interactable = false;

            // read session password and 
            counterbalancingList = ReadPasswords();
        }

        private Dictionary<string, Counterbalancing> ReadPasswords()
        {
            TextAsset rawData = Resources.Load<TextAsset>(sessionInfoFilePath);

            return rawData.text
                .Split('\n') // split by new line
                .Skip(1) // skip first line to avoid reading headers
                .Where(ln => ln != "") // exclude blank lines 
                .Select(ln => ln.Split(',')) // delimit by comma
                .ToDictionary(r => r[0].ToString(), r => new Counterbalancing(){ // return as dictionary
                    SID = int.Parse(r[1]),
                    TaskOrder = (TaskOrderType)int.Parse(r[2]),
                    QuestionnaireOrder = (QuestionnaireOrderType)int.Parse(r[3])
                });
        }

        public void OnPasswordValueChange()
        {
            string currentPassword = PasswordInputField.text;
            if (counterbalancingList.ContainsKey(currentPassword))
            {
                counterbalancing = counterbalancingList[currentPassword];

                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    FirebaseDatabase.GetJSON($"{Global.FirebasePath}/SID{counterbalancing.SID}/Started",
                        gameObject.name,
                        "GetJSONCallback",
                        "");
                }

                UnlockButton.interactable = true;
            }
        }

        public void OnUnlockClick()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                FirebaseDatabase.GetJSON($"{Global.FirebasePath}/SID{counterbalancing.SID}/Started",
                    gameObject.name,
                    "GetJSONCallback",
                    ""); 
                if (jsonData != "no data")
                {
                    return;
                }
            }

            Screen.fullScreen = true;

            Global.SessionInfo.StartTime = System.DateTime.Now;
            Global.SessionInfo.SID = counterbalancing.SID;
            Global.SessionInfo.TaskOrder = counterbalancing.TaskOrder;
            Global.SessionInfo.QuestionnaireOrder = counterbalancing.QuestionnaireOrder;
            Global.SessionInfo.Block = 0;
            Global.SessionInfo.IsPractice = true;

            this.gameObject.SetActive(false);
            NextPanel.SetActive(true);
        }

        [HideInInspector]
        public void GetJSONCallback(string data) => jsonData = (data == "null" || data == null || data == "") ? "no data" : data;
    }
}