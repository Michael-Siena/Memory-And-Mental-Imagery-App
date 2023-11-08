using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CustomDataTypes;

namespace UI.Instructions
{
    public sealed class QuestionnaireInstructions : Instructions
    {
        protected override void OnEnable() => base.OnEnable();

        public override void OnStartButtonClick()
        {
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
        }

        protected override void OnDisable() => base.OnDisable();
    }
}
