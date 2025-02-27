using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SLC_GameJam_2025_1
{
    [RequireComponent(typeof (CanvasGroup))]
    public class OnBoardingUI : MonoBehaviour
    {
        public float m_stallDuration = 0.5f;
        public float m_fadeInDuration = 1f;
        public float m_fadeOutDuration = 1f;
        private CanvasGroup m_canvasGroup;
        private bool m_isComplete;
        private float m_fadeTime;
        private readonly UnityEvent m_onComplete = new();
        public event Action onFinished;
        private bool m_isInProgress;
        private bool m_isCompletionQueued;

        private void Awake()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
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
        
        private IEnumerator Fade(float fadeTime, float start, float end)
        {
            m_fadeTime = 0;
            
            while (true)
            {
                m_canvasGroup.alpha = start;
                m_fadeTime += Time.deltaTime / fadeTime;
                m_canvasGroup.alpha = Mathf.Lerp(start, end, m_fadeTime);
                
                if (m_fadeTime >= 1)
                {
                    m_canvasGroup.alpha = end;
                    break;
                }
                
                yield return null;
            }
        }
    }
}
