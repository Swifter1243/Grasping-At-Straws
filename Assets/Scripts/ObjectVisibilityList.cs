using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SLC_GameJam_2025_1
{
    public class ObjectVisibilityList : MonoBehaviour
    {
        public List<RectTransform> m_objects = new();
        private string m_lastObjectName;
        private readonly Dictionary<string, RectTransform> m_hashedObjects = new();

        public void Initialize()
        {
            m_lastObjectName = m_objects[0].name;
            foreach (RectTransform rectTransform in m_objects)
            {
                rectTransform.gameObject.SetActive(false);
                m_hashedObjects[rectTransform.name] = rectTransform;
            }
        }

        public void SetVisible(string objectName)
        {
            m_hashedObjects[m_lastObjectName].gameObject.SetActive(false);
            m_lastObjectName = objectName;
            m_hashedObjects[objectName].gameObject.SetActive(true);
        }
    }
}
