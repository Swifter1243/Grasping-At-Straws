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

			m_inUseGizmo.RaycastClickPosition(ray);
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

		public void Open(PuzzlePiece piece)
		{
			gameObject.SetActive(true);
			transform.position = piece.transform.position;
		}

		public void Close()
		{
			gameObject.SetActive(false);
		}
	}
}
