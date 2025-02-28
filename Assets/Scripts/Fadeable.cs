using System;
using System.Collections;
using UnityEngine;

namespace SLC_GameJam_2025_1
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class Fadeable : MonoBehaviour
    {
        private CanvasGroup m_canvasGroup;
        private float m_fadeTime;

        protected virtual void Awake()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
        }

        protected IEnumerator Fade(float fadeTime, float start, float end)
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
