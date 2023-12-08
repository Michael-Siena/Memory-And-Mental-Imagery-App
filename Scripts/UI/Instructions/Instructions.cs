using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UI.Instructions
{
    public abstract class Instructions : MonoBehaviour
    {
        public GameObject[] Panels
        {
            get => _panels;
            protected set => _panels = value;
        }
        [SerializeField]
        protected GameObject[] _panels;
        protected int iPanel;

        protected virtual void OnEnable()
        {
            // set start panel to active and disable all other panels
            Panels[0].SetActive(true);
            foreach (GameObject panel in Panels.Skip(1))
            {
                panel.SetActive(false);
            }

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void OnNextButtonClick()
        {
            Panels[iPanel].SetActive(false);
            Panels[++iPanel].SetActive(true);
        }

        public void OnPrevButtonClick()
        {
            Panels[iPanel].SetActive(false);
            Panels[--iPanel].SetActive(true);
        }

        public abstract void OnStartButtonClick();

        protected virtual void OnDisable()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}