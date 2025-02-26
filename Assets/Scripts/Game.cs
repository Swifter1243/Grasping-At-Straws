using System;
using System.Collections;
using UnityEngine;

namespace SLC_GameJam_2025_1
{
    public class Game : MonoBehaviour
    {
        public PuzzleLayout m_puzzleLayout;
        public ParticleSystem m_leakParticles;

        private const float FOCUSED_OPACITY = 1;
        private const float UNFOCUSED_OPACITY = 0.2f;
        
        private bool m_solving = false;
        private float m_pipeProgress = 0;

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
            m_solving = true;
            m_puzzleLayout.SetAllPipesOpacity(UNFOCUSED_OPACITY);
            
            if (solution.m_first == null)
            {
                PlaceLeak(m_puzzleLayout.m_in.BoardPosition, m_puzzleLayout.m_in.BoardDirection);
            }
            else
            {
                ProcessNextEntry(solution, solution.m_first);
            }
        }

        private IEnumerator AnimatePipe(PuzzleSolution solution, PuzzleSolution.Entry entry)
        {
            m_pipeProgress = 0;
            PuzzlePiece piece = entry.m_piece;
            piece.SetFluidFlipped(entry.m_enteredInput1);
            
            while (true)
            {
                m_pipeProgress += Time.deltaTime * 0.6f;
                piece.SetFluidProgress(m_pipeProgress);

                float opacity = Mathf.Lerp(UNFOCUSED_OPACITY, FOCUSED_OPACITY, Math.Min(1, m_pipeProgress * 4));
                piece.SetOpacity(opacity);

                if (m_pipeProgress >= 1f)
                {
                    piece.SetFluidProgress(m_pipeProgress);
                    ProcessNextEntry(solution, entry.m_next);
                    break;
                }

                yield return null;
            }
        }

        private void ProcessNextEntry(PuzzleSolution solution, PuzzleSolution.Entry nextEntry)
        {
            if (nextEntry == null)
            {
                m_solving = false;
                
                if (solution.m_success)
                {
                    Debug.Log("Success!");
                }
                else
                {
                    PlaceLeak(solution.m_last.m_piece.BoardPosition, solution.m_last.m_directionOut);
                }
            }
            else
            {
                StartCoroutine(AnimatePipe(solution, nextEntry));
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
