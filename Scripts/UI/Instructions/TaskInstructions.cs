using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UI.Instructions
{
    public sealed class TaskInstructions : Instructions
    {
        protected override void OnEnable() => base.OnEnable();

        public int NextSceneInBuildIndex
        {
            get => _nextSceneInBuildIndex;
            private set => _nextSceneInBuildIndex = value;
        }
        [SerializeField]
        private int _nextSceneInBuildIndex;

        public override void OnStartButtonClick()
        {
            SceneManager.LoadScene(NextSceneInBuildIndex);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;

            Screen.fullScreen = true;
        }

        protected override void OnDisable() => base.OnDisable();
    }
}