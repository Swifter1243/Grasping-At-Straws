using System;
using UnityEngine;

namespace SLC_GameJam_2025_1
{
    public class SceneInteraction : MonoBehaviour
    {
        private Camera m_camera;
        public Game m_game;
        public GizmoHandler m_gizmoHandler;

        private void Awake()
        {
            m_camera = Camera.main;
        }

        private void Update()
        {
            if (m_game.m_state == Game.State.Editing)
            {
                DoInteraction();
            }
        }

        private void DoInteraction()
        {
            if (!m_camera)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                OnLeftClick();
            }
            if (Input.GetMouseButton(0))
            {
                OnLeftClickHeld();
            }
            if (Input.GetMouseButtonUp(0))
            {
                OnLeftClickReleased();
            }
        }

        private void OnLeftClickHeld()
        {
            m_gizmoHandler.UpdateGizmoInteraction(GetMouseRay());
        }

        private void OnLeftClickReleased()
        {
            m_gizmoHandler.StopUsing();
        }

        private Ray GetMouseRay()
        {
            return m_camera.ScreenPointToRay(Input.mousePosition);
        }

        private void OnLeftClick()
        {
            if (Physics.Raycast(GetMouseRay(), out RaycastHit hit, 100))
            {
                OnHit(hit);
            }
            else
            {
                m_game.DeselectPiece();
            }
        }

        private void OnHit(RaycastHit hit)
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Pipe"))
            {
                PuzzlePiece piece = hit.transform.GetComponent<PuzzlePiece>();
                m_game.SelectPiece(piece);
            }
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Rotation Gizmo"))
            {
                RotationGizmo rotationGizmo = hit.transform.GetComponent<RotationGizmo>();
                m_gizmoHandler.StartUsing(rotationGizmo);
            }
        }
    }
}
