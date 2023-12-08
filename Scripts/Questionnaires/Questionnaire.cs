using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FirebaseWebGL;

namespace Questionnaires
{
    public abstract class Questionnaire : MonoBehaviour
    {
        public GameObject[] Panels;
        protected int iPanel;

        protected TMP_Dropdown[] currentDropdownMenus;
        protected List<int> currentDataChunk;

        public GameObject Tooltip;

        // contains a list of int lists for each set of dropdown menus
        protected List<int> data;

        protected DateTime startDateTime, finishDateTime;

        protected virtual void OnEnable()
        {
            data = new List<int>();

            // set start panel to active and disable all other panels
            Panels[0].SetActive(true);
            foreach (GameObject panel in Panels.Skip(1))
            {
                panel.SetActive(false);
            }

            iPanel = 0;

            Tooltip.SetActive(false);

            Screen.fullScreen = true;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public virtual void OnStartButtonPress()
        {
            startDateTime = DateTime.Now;

            Panels[iPanel].SetActive(false);
            Panels[++iPanel].SetActive(true);
        }

        public virtual void OnNextButtonPress()
        {
            // checks that all items have been given ratings before moving onto next panel
            currentDropdownMenus = Panels[iPanel].GetComponentsInChildren<TMP_Dropdown>();
            if (currentDropdownMenus.Length > 0)
            {
                for (int i = 0; i < currentDropdownMenus.Length; i++)
                {
                    // we show tooltip and break out of method before assigning new data if any
                    // dropdown menus are empty
                    if (currentDropdownMenus[i].value == 0)
                    {
                        Tooltip.SetActive(true);
                        return;
                    }
                }
            }

            Panels[iPanel].SetActive(false);
            Panels[++iPanel].SetActive(true);
        }

        public virtual void OnPrevButtonPress()
        {
            Panels[iPanel].SetActive(false);
            Panels[--iPanel].SetActive(true);

            Tooltip.SetActive(false);
        }

        public virtual void OnSubmitButtonPress()
        {
            finishDateTime = DateTime.Now;

            Panels[iPanel].SetActive(false);

            for (int i = 0; i < Panels.Length; i++)
            {
                TMP_Dropdown[] dropdownMenus = Panels[i].GetComponentsInChildren<TMP_Dropdown>();
                if (dropdownMenus.Length > 0)
                {
                    for (int j = 0; j < dropdownMenus.Length; j++)
                    {
                        data.Add(dropdownMenus[j].value);
                    }
                }
            }

            string formattedData = ConvertDataToString(data);

            // we don't use any callbacks when posting to db
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                FirebaseDatabase.PostJSON($"{Global.FirebasePath}/SID{Global.SessionInfo.SID}/{this.gameObject.name}/Ratings", $"{startDateTime}, {formattedData}, {finishDateTime}", this.gameObject.name, "", "");
            }

            Debug.Log($"{Global.FirebasePath}/SID{Global.SessionInfo.SID}/{this.gameObject.name}/Ratings");
            Debug.Log(data.Count);
            Debug.Log(formattedData);

            LoadNextScene();
        }

        protected virtual string ConvertDataToString(List<int> data)
        {
            var sb = new StringBuilder(data.First().ToString());
            foreach (int d in data.Skip(1))
            {
                sb.Append($", {d}");
            }

            return sb.ToString();
        }

        // change scene based on session info counterbalancing global static var
        // use counterbalancing to select next scene
        protected abstract void LoadNextScene();

        protected virtual void OnDisable()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}