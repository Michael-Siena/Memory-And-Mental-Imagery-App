using UnityEngine;

namespace TrialStages
{
    public sealed class FixationCross : TrialStage
    {
        public GameObject Panel
        {
            get => _panel;
            private set => _panel = value;
        }
        [SerializeField]
        private GameObject _panel;

        protected override void OnEnable()
        {
            Panel.SetActive(true);

            base.OnEnable();
        }

        protected override void Update() => base.Update();

        protected override void OnDisable()
        {
            Panel.SetActive(false);

            base.OnDisable();
        }
    }
}