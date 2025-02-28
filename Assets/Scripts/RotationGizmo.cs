using System;
using UnityEngine;
namespace SLC_GameJam_2025_1
{
	public class RotationGizmo : MonoBehaviour
	{
		public float m_rotationSensitivity = 0.4f;
		public MeshRenderer m_meshRenderer;
		private static readonly int s_inUse = Shader.PropertyToID("_InUse");
		private static readonly int s_opacity = Shader.PropertyToID("_Opacity");
		private static readonly int s_axisColor = Shader.PropertyToID("_AxisColor");
		private static readonly int s_clickPosition = Shader.PropertyToID("_ClickPosition");
		private readonly static int s_visibility = Shader.PropertyToID("_Visibility");
		private PuzzlePiece m_activePiece;
		private Vector2 m_startingClickPosition;
		private float m_lastAngle;
		public event Action onRotationMade;
		public Collider m_collider;
		private Camera m_camera;

		public void Initialize(Color axisColor)
		{
			m_meshRenderer.material.SetColor(s_axisColor, axisColor);
			m_meshRenderer.material.SetFloat(s_opacity, 1);
			m_meshRenderer.material.SetFloat(s_inUse, 0);
			m_camera = Camera.main;
		}

		private void Update()
		{
			if (!m_camera)
				return;

			Vector3 cameraForward = m_camera.transform.forward;
			float camAngle = Vector3.Dot(transform.up, cameraForward);
			float visibility = Math.Abs(camAngle);

			float threshold = 0.3f;
			m_collider.enabled = visibility > threshold;
			float materialVisibility = Mathf.Clamp01(Mathf.InverseLerp(threshold - 0.1f, threshold + 0.1f, visibility));
			m_meshRenderer.material.SetFloat(s_visibility, materialVisibility);
		}

		public void SetActivePiece(PuzzlePiece piece)
		{
			m_activePiece = piece;
		}

		public void SetInUse(Ray ray)
		{
			m_meshRenderer.material.SetInt(s_inUse, 1);
			Vector2 clickPosition = GetClickPosition(ray) ?? Vector2.zero;
			m_startingClickPosition = clickPosition;
			m_lastAngle = 0;
		}

		private float GetAngleFromClickPosition(Vector2 clickPosition)
		{
			return Mathf.Atan2(clickPosition.y, clickPosition.x);
		}

		private float SnapAngleTo90(float angle)
		{
			return Mathf.Floor(angle / 90f + 0.5f) * 90f;
		}

		private Vector2? GetClickPosition(Ray ray)
		{
			Plane plane = new(transform.up, transform.position);
			if (plane.Raycast(ray, out float enter))
			{
				Vector3 worldPoint = ray.GetPoint(enter);
				Vector3 localPoint = transform.InverseTransformPoint(worldPoint);
				return new Vector2(localPoint.x, localPoint.z);
			}
			return null;
		}

		public void SetUnfocused()
		{
			m_meshRenderer.material.SetFloat(s_opacity, 0.2f);
		}

		private void SetClickPosition(Vector2 clickPosition)
		{
			Vector4 vector = new(clickPosition.x, clickPosition.y, 0, 0);
			m_meshRenderer.material.SetVector(s_clickPosition, vector);
		}

		private float Project(Vector2 a, Vector2 b)
		{
			return Vector2.Dot(a, b) / Vector2.Dot(b, b);
		}

		public void UpdateClickPosition(Ray ray)
		{
			Vector2 clickPosition = GetClickPosition(ray) ?? m_startingClickPosition;
			Vector2 tangent = new(-m_startingClickPosition.y, m_startingClickPosition.x);

			float signedDistance = Project(clickPosition - m_startingClickPosition, tangent);
			float angle01 = signedDistance / m_rotationSensitivity;
			float angle = angle01 * 90;

			float a = m_lastAngle;
			float b = SnapAngleTo90(angle);
			m_lastAngle = b;

			float deltaAngle = b - a;

			const float HALF_PI = Mathf.PI * 0.5f;
			float targetAngle = GetAngleFromClickPosition(m_startingClickPosition) + angle01 * HALF_PI;
			SetClickPosition(new Vector2(Mathf.Cos(targetAngle), Mathf.Sin(targetAngle)));

			if (deltaAngle != 0)
			{
				UpdateRotation(deltaAngle);
			}
		}

		private void UpdateRotation(float deltaAngle)
		{
			m_activePiece.transform.rotation = Quaternion.AngleAxis(-deltaAngle, transform.up) * m_activePiece.transform.rotation;
			onRotationMade?.Invoke();
		}

		public void ResetVisuals()
		{
			m_meshRenderer.material.SetInt(s_inUse, 0);
			m_meshRenderer.material.SetFloat(s_opacity, 1);
		}
	}
}
