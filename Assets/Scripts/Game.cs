using System;
using System.Collections;
using UnityEngine;

namespace SLC_GameJam_2025_1
{
    public class Game : MonoBehaviour
    {
        public PuzzleLayout m_puzzleLayout;
        public ParticleSystem m_leakParticles;
        public CameraSmoothing m_cameraSmoothing;

        private const float FOCUSED_OPACITY = 1;
        private const float UNFOCUSED_OPACITY = 0.2f;

        public enum State
        {
            Editing,
            Solving,
            ViewingResult
        }

        public State m_state = State.Editing;
        private float m_pipeProgress = 0;
        private PuzzlePiece m_selectedPiece = null;

        public void SelectPiece(PuzzlePiece piece)
        {
            DeselectPiece();
            piece.Select();
            m_selectedPiece = piece;
            m_cameraSmoothing.m_targetPivot = piece.transform.position;
            m_cameraSmoothing.m_targetDistance = 2;
        }

        public void DeselectPiece()
        {
            if (m_selectedPiece != null)
            {
                ResetView();
                m_selectedPiece.Deselect();
                m_selectedPiece = null;
            }
        }

        private void ResetLevel()
        {
            m_leakParticles.gameObject.SetActive(false);
        }

        private void Awake()
        {
            m_puzzleLayout.Initialize();
            ResetLevel();
            ResetView();
        }

        private void ResetView()
        {
            m_cameraSmoothing.SetFromBounds(m_puzzleLayout.BoundingBox);
        }

        private void AttemptSolve()
        {
            DeselectPiece();
            PuzzleSolution solution = m_puzzleLayout.Solve();
            m_state = State.Solving;
            m_puzzleLayout.SetAllPipesOpacity(UNFOCUSED_OPACITY);

            if (solution.m_first == null)
            {
                OnSolveFailure(m_puzzleLayout.m_in.BoardPosition, m_puzzleLayout.m_in.BoardDirection);
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
                m_state = State.ViewingResult;

                if (solution.m_success)
                {
                    OnSolveSuccess();
                }
                else
                {
                    OnSolveFailure(solution.m_last.m_piece.BoardPosition, solution.m_last.m_directionOut);
                }
            }
            else
            {
                StartCoroutine(AnimatePipe(solution, nextEntry));
            }
        }

        private void OnSolveSuccess()
        {
            
        }

        private void OnSolveFailure(Vector3Int position, Vector3Int direction)
        {
            PlaceLeak(position, direction);
        }

        private void PlaceLeak(Vector3Int position, Vector3Int direction)
        {
            Vector3 pos = position + (Vector3) direction * 0.5f;
            m_leakParticles.transform.position = pos;
            m_leakParticles.transform.rotation = Quaternion.LookRotation(direction);
            m_leakParticles.gameObject.SetActive(true);
        }
    }
}
