using System;
using UnityEngine;
namespace SLC_GameJam_2025_1
{
	public class RotationGizmo : MonoBehaviour
	{
		public MeshRenderer m_meshRenderer;
		private static readonly int s_inUse = Shader.PropertyToID("_InUse");
		private static readonly int s_opacity = Shader.PropertyToID("_Opacity");
		private static readonly int s_axisColor = Shader.PropertyToID("_AxisColor");
		private static readonly int s_clickPosition = Shader.PropertyToID("_ClickPosition");
		private float m_startAngle;
		private PuzzlePiece m_activePiece;
		private Quaternion m_startRotation;

		public void Initialize(Color axisColor)
		{
			m_meshRenderer.material.SetColor(s_axisColor, axisColor);
			m_meshRenderer.material.SetFloat(s_opacity, 1);
			m_meshRenderer.material.SetFloat(s_inUse, 0);
		}

		public void SetActivePiece(PuzzlePiece piece)
		{
			m_activePiece = piece;
		}

		public void SetInUse(Ray ray)
		{
			m_startRotation = m_activePiece.transform.rotation;
			m_meshRenderer.material.SetInt(s_inUse, 1);
			Vector2 clickPosition = GetClickPosition(ray);
			m_startAngle = GetAngleFromClickPosition(clickPosition);
		}

		private float GetAngleFromClickPosition(Vector2 clickPosition)
		{
			return Mathf.Atan2(clickPosition.y, clickPosition.x) * Mathf.Rad2Deg;
		}

		private Vector2 GetClickPosition(Ray ray)
		{
			Plane plane = new(transform.up, transform.position);
			if (plane.Raycast(ray, out float enter))
			{
				Vector3 worldPoint = ray.GetPoint(enter);
				Vector3 localPoint = transform.InverseTransformPoint(worldPoint);
				return new Vector2(localPoint.x, localPoint.z);
			}
			return Vector2.zero;
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

		public void UpdateClickPosition(Ray ray)
		{
			Vector2 clickPosition = GetClickPosition(ray);
			SetClickPosition(clickPosition);
			float angle = GetAngleFromClickPosition(clickPosition);
			float deltaAngle = angle - m_startAngle;
			UpdateRotation(Mathf.Round(deltaAngle / 90) * 90);
		}

		private void UpdateRotation(float angle)
		{
			m_activePiece.transform.rotation = Quaternion.AngleAxis(-angle, transform.up) * m_startRotation;
		}

		public void ResetVisuals()
		{
			m_meshRenderer.material.SetInt(s_inUse, 0);
			m_meshRenderer.material.SetFloat(s_opacity, 1);
		}
	}
}
