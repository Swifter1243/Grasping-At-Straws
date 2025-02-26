using System;
using UnityEngine;

namespace SLC_GameJam_2025_1
{
    public class Level : MonoBehaviour
    {
        public PuzzleLayout m_puzzleLayout;
        public ParticleSystem m_leakParticles;

        private void RestartLevel()
        {
            m_leakParticles.gameObject.SetActive(false);
        }

        private void Awake()
        {
            m_puzzleLayout.Initialize();
            RestartLevel();
            
            AttemptSolve();
        }

        private void AttemptSolve()
        {
            PuzzleSolution solution = m_puzzleLayout.Solve();

            if (solution.m_first == null)
            {
                PlaceLeak(m_puzzleLayout.m_in.BoardPosition, m_puzzleLayout.m_in.BoardDirection);
            }
            else
            {
                if (solution.m_success)
                {
                    Debug.Log("Success!");
                }
                else
                {
                    PlaceLeak(solution.m_last.m_piece.BoardPosition, solution.m_last.m_directionOut);
                }
            }
        }

        private void PlaceLeak(Vector3Int position, Vector3Int direction)
        {
            Vector3 pos = position + (Vector3)direction * 0.5f;
            m_leakParticles.transform.position = pos;
            m_leakParticles.transform.rotation = Quaternion.LookRotation(direction);
            m_leakParticles.gameObject.SetActive(true);
        }
    }
}
