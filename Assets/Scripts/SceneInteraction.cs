using System;
using UnityEngine;

namespace SLC_GameJam_2025_1
{
    public class SceneInteraction : MonoBehaviour
    {
        private Camera m_camera;
        public Game m_game;

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
        }

        private void OnLeftClick()
        {
            Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100))
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
        }
    }
}
