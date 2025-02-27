using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SLC_GameJam_2025_1
{
    public class ObjectVisibilityList : MonoBehaviour
    {
        public float m_transitionOutTime = 0.5f;
        public float m_transitionInTime = 0.5f;
        public List<CanvasGroup> m_objects = new();
        private CanvasGroup m_lastObject;
        private CanvasGroup m_currentObject;
        private bool m_isTransitioning;
        private readonly Dictionary<string, CanvasGroup> m_hashedObjects = new();

        public void Initialize()
        {
            foreach (CanvasGroup canvasGroup in m_objects)
            {
                canvasGroup.gameObject.SetActive(false);
                canvasGroup.alpha = 0;
                m_hashedObjects[canvasGroup.gameObject.name] = canvasGroup;
            }
        }

        public void SetVisible(string objectName)
        {
            if (m_isTransitioning)
            {
                m_lastObject.gameObject.SetActive(false);
                m_lastObject.alpha = 0;
                m_lastObject.gameObject.SetActive(true);
                m_currentObject.alpha = 1;
                m_isTransitioning = false;
                StopAllCoroutines();
            }

            m_lastObject = m_currentObject;
            m_currentObject = m_hashedObjects[objectName];
            StartCoroutine(Transition(m_lastObject, m_currentObject));
        }

        private IEnumerator Transition(CanvasGroup fadeOutObj, CanvasGroup fadeInObj)
        {
            m_isTransitioning = true;
            if (fadeOutObj)
            {
                yield return Fade(m_transitionOutTime, 1, 0, fadeOutObj);
                fadeOutObj.gameObject.SetActive(false);
            }
            fadeInObj.gameObject.SetActive(true);
            yield return Fade(m_transitionInTime, 0, 1, fadeInObj);
            m_isTransitioning = false;
        }

        private float m_fadeTime;
        
        private IEnumerator Fade(float fadeTime, float start, float end, CanvasGroup obj)
        {
            m_fadeTime = 0;
            
            while (true)
            {
                obj.alpha = start;
                m_fadeTime += Time.deltaTime / fadeTime;
                obj.alpha = Mathf.Lerp(start, end, m_fadeTime);
                
                if (m_fadeTime >= 1)
                {
                    obj.alpha = end;
                    break;
                }
                
                yield return null;
            }
        }
    }
}
