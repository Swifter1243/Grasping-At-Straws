using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SLC_GameJam_2025_1
{
	public class GizmoHandler : MonoBehaviour
	{
		public RotationGizmo m_xAxis;
		public RotationGizmo m_yAxis;
		public RotationGizmo m_zAxis;

		private RotationGizmo m_inUseGizmo;

		private IEnumerable<RotationGizmo> GetRotationGizmos()
		{
			yield return m_xAxis;
			yield return m_yAxis;
			yield return m_zAxis;
		}

		private void Awake()
		{
			m_xAxis.Initialize(Color.red);
			m_yAxis.Initialize(Color.green);
			m_zAxis.Initialize(Color.blue);
		}

		public void StartUsing(RotationGizmo rotationGizmo)
		{
			foreach (RotationGizmo gizmo in GetRotationGizmos())
			{
				gizmo.SetUnfocused();
			}

			m_inUseGizmo = rotationGizmo;
			rotationGizmo.SetInUse();
		}

		public void UpdateGizmoInteraction(Ray ray)
		{
			if (!m_inUseGizmo)
				return;

			if (!Physics.BoxCast(m_inUseGizmo.transform.position, m_inUseGizmo.transform.up, ray.direction, out RaycastHit hit))
				return;

			Vector3 worldPoint = m_inUseGizmo.transform.InverseTransformPoint(hit.point);
			Vector2 clickPosition = new(worldPoint.x, worldPoint.z);
			m_inUseGizmo.SetClickPosition(clickPosition);
		}

		public void StopUsing()
		{
			if (!m_inUseGizmo)
				return;

			foreach (RotationGizmo gizmo in GetRotationGizmos())
			{
				gizmo.ResetVisuals();
			}
			m_inUseGizmo = null;
		}
	}
}
