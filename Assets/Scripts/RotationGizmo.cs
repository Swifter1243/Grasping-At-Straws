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

		public void Initialize(Color axisColor)
		{
			m_meshRenderer.material.SetColor(s_axisColor, axisColor);
			m_meshRenderer.material.SetFloat(s_opacity, 1);
			m_meshRenderer.material.SetFloat(s_inUse, 0);
		}

		public void SetInUse()
		{
			m_meshRenderer.material.SetInt(s_inUse, 1);
		}

		public void SetUnfocused()
		{
			m_meshRenderer.material.SetFloat(s_opacity, 0.2f);
		}

		public void SetClickPosition(Vector2 clickPosition)
		{
			Vector4 vector = new(clickPosition.x, clickPosition.y, 0, 0);
			m_meshRenderer.material.SetVector(s_clickPosition, vector);
		}

		public void ResetVisuals()
		{
			m_meshRenderer.material.SetInt(s_inUse, 0);
			m_meshRenderer.material.SetFloat(s_opacity, 1);
		}
	}
}
