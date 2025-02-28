using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SLC_GameJam_2025_1
{
    public class OnBoardingUI : Fadeable
    {
        public float m_fadeInDuration = 1f;
        public float m_fadeOutDuration = 1f;
        public float m_stallDuration = 0.5f;
        private bool m_isComplete;
        private readonly UnityEvent m_onComplete = new();
        public event Action onFinished;
        private bool m_isInProgress;
        private bool m_isCompletionQueued;

        protected override void Awake()
        {
            base.Awake();
            m_onComplete.AddListener(OnComplete);
        }

        private void OnComplete()
        {
            if (m_isInProgress)
            {
                m_isCompletionQueued = true;
                return;
            }
            
            if (m_isComplete)
                return;

            m_isComplete = true;
            StartCoroutine(Finish());
        }

        public UnityEvent Setup()
        {
            StartCoroutine(StartOnboarding());
            
            return m_onComplete;
        }

        private IEnumerator StartOnboarding()
        {
            m_isInProgress = true;
            yield return new WaitForSeconds(m_stallDuration);
            yield return Fade(m_fadeInDuration, 0, 1);
            m_isInProgress = false;

            if (m_isCompletionQueued)
            {
                m_isCompletionQueued = false;
                OnComplete();
            }
        }

        private IEnumerator Finish()
        {
            yield return Fade(m_fadeOutDuration, 1, 0);
            onFinished?.Invoke();
        }
    }
}
