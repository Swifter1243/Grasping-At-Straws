using System;
using UnityEngine;

namespace SLC_GameJam_2025_1
{
    public class CameraSmoothing : MonoBehaviour
    {
        public Vector3 m_initialRotation;
        public float m_smoothRate = 1;
        public float m_targetDistance = 3;
        private Quaternion m_targetRotation = Quaternion.identity;
        public Vector3 m_targetPivot = Vector3.zero;
        private float m_distance = 0;
        private Vector3 m_pivot = Vector3.zero;
        private Quaternion m_rotation = Quaternion.identity;

        // https://www.gamedeveloper.com/programming/improved-lerp-smoothing-
        private static float LerpSmooth(float target, float value, float dt, float rate)
        {
            return Mathf.Lerp(target, value, Mathf.Exp(-rate*dt));
        }
        private static Vector3 LerpSmooth(Vector3 target, Vector3 value, float dt, float rate)
        {
            return Vector3.Lerp(target, value, Mathf.Exp(-rate*dt));
        }
        private static Quaternion LerpSmooth(Quaternion target, Quaternion value, float dt, float rate)
        {
            return Quaternion.Slerp(target, value, Mathf.Exp(-rate*dt));
        }

        private void Awake()
        {
            m_targetRotation = Quaternion.Euler(m_initialRotation);
        }

        private void Update()
        {
            m_distance = LerpSmooth(m_targetDistance, m_distance, Time.deltaTime, m_smoothRate);
            m_pivot = LerpSmooth(m_targetPivot, m_pivot, Time.deltaTime, m_smoothRate);
            m_rotation = LerpSmooth(m_targetRotation, m_rotation, Time.deltaTime, m_smoothRate);

            Vector3 back = m_rotation * Vector3.back;
            transform.rotation = m_rotation;
            transform.position = m_pivot + back * m_distance;
        }

        public void SetFromBounds(Bounds bounds)
        {
            m_targetDistance = bounds.extents.magnitude * 1.5f;
            m_targetPivot = bounds.center;
        }
    }
}
