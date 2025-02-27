using System;
using UnityEngine;

namespace SLC_GameJam_2025_1
{
    public class CameraSmoothing : MonoBehaviour
    {
        public Vector3 m_initialRotation;
        public float m_smoothRate = 1;
        public Vector2 m_lookSensitivity = new(500, 300);
        public float m_scrollSensitivity = 1;

        public float m_targetDistance = 3;
        private Quaternion m_targetRotation = Quaternion.identity;
        public Vector3 m_targetPivot = Vector3.zero;

        private float m_distanceFactor = 1;
        private float m_distance = 0;
        private Vector3 m_pivot = Vector3.zero;
        private Quaternion m_rotation = Quaternion.identity;
        private Vector3 m_lastMousePosition = Vector3.zero;

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
            m_distance = LerpSmooth(m_targetDistance * m_distanceFactor, m_distance, Time.deltaTime, m_smoothRate);
            m_pivot = LerpSmooth(m_targetPivot, m_pivot, Time.deltaTime, m_smoothRate);
            m_rotation = LerpSmooth(m_targetRotation, m_rotation, Time.deltaTime, m_smoothRate);

            Vector3 back = m_rotation * Vector3.back;
            transform.rotation = m_rotation;
            transform.position = m_pivot + back * m_distance;

            if (Input.GetMouseButtonDown(1))
            {
                m_lastMousePosition = Input.mousePosition;
            }

            if (Input.mouseScrollDelta.y != 0)
            {
                m_distanceFactor -= Input.mouseScrollDelta.y * m_scrollSensitivity;
                m_distanceFactor = Mathf.Clamp(m_distanceFactor, 0.4f, 2);
            }

            if (Input.GetMouseButton(1))
            {
                Vector3 mouseDelta = Input.mousePosition - m_lastMousePosition;
                m_lastMousePosition = Input.mousePosition;

                Vector2 movement = mouseDelta.normalized * (Time.deltaTime * m_lookSensitivity);

                m_targetRotation = Quaternion.Euler(0, movement.x, 0) * m_targetRotation;
                m_targetRotation *= Quaternion.Euler(-movement.y, 0, 0);
            }
        }

        public void SetFromBounds(Bounds bounds)
        {
            m_targetDistance = bounds.GetMaxAxis() * 3.5f;
            m_targetPivot = bounds.center;
        }
    }
}
