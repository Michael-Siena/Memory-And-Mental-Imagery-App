using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrialStages
{
    public abstract class TrialStage : MonoBehaviour, ITimeoutable
    {
        public delegate void FinishedAction();
        public static event FinishedAction OnFinished;

        public float Duration
        {
            get => _duration;
            protected set => _duration = value;
        }
        [SerializeField]
        private float _duration; // exposed to the inspector
        public float TimeLeft { get; protected set; }

        protected virtual void OnEnable()
        {
            TimeLeft = Duration;
        }

        protected virtual void Update()
        {
            if (HasTimedOut())
            {
                this.gameObject.SetActive(false);
            }
        }

        public bool HasTimedOut()
        {
            TimeLeft -= Time.deltaTime;
            return (TimeLeft <= 0f);
        }

        protected virtual void OnDisable() => OnFinished?.Invoke();
    }
}