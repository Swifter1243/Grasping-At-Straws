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
        public ObjectVisibilityList m_uiLayouts;

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
        private PuzzleSolution m_currentSolution = null;

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

        public void StartEditing()
        {
            m_state = State.Editing;
            m_uiLayouts.SetVisible("Editor UI");
            m_leakParticles.gameObject.SetActive(false);
            
            foreach (PuzzlePiece puzzlePiece in m_puzzleLayout)
            {
                puzzlePiece.SetOpacity(FOCUSED_OPACITY);
                puzzlePiece.SetFluidProgress(0);
            }
        }

        private void Awake()
        {
            m_puzzleLayout.Initialize();
            StartEditing();
            ResetView();
        }

        private void ResetView()
        {
            m_cameraSmoothing.SetFromBounds(m_puzzleLayout.BoundingBox);
        }

        public void FastForward()
        {
            if (m_state == State.Solving)
            {
                StopAllCoroutines();

                PuzzleSolution.Entry entry = m_currentSolution.m_first;
                while (!CheckSolutionFinished(entry))
                {
                    entry.m_piece.SetOpacity(FOCUSED_OPACITY);
                    entry.m_piece.SetFluidProgress(1);
                    entry = entry.m_next;
                }
            }
        }

        public void AttemptSolve()
        {
            if (m_state == State.ViewingResult)
            {
                StartEditing();
                AttemptSolve();
            }
            if (m_state == State.Editing)
            {
                m_uiLayouts.SetVisible("Solving UI");
                DeselectPiece();
                m_currentSolution = m_puzzleLayout.Solve();
                m_state = State.Solving;
                m_puzzleLayout.SetAllPipesOpacity(UNFOCUSED_OPACITY);

                if (m_currentSolution.m_first == null)
                {
                    OnSolveFailure(m_puzzleLayout.m_in.BoardPosition, m_puzzleLayout.m_in.BoardDirection);
                }
                else
                {
                    ProcessNextAnimatedPipe(m_currentSolution.m_first);
                }
            }
        }

        private void ProcessNextAnimatedPipe(PuzzleSolution.Entry nextEntry)
        {
            if (!CheckSolutionFinished(nextEntry))
            {
                StartCoroutine(AnimatePipe(nextEntry));
            }
        }

        private IEnumerator AnimatePipe(PuzzleSolution.Entry entry)
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
                    ProcessNextAnimatedPipe(entry.m_next);
                    break;
                }

                yield return null;
            }
        }

        private bool CheckSolutionFinished(PuzzleSolution.Entry nextEntry)
        {
            if (nextEntry != null)
                return false;

            if (m_currentSolution.m_success)
            {
                OnSolveSuccess();
            }
            else
            {
                OnSolveFailure(m_currentSolution.m_last.m_piece.BoardPosition, m_currentSolution.m_last.m_directionOut);
            }

            return true;
        }

        private void OnSolveSuccess()
        {
            m_state = State.ViewingResult;
            m_uiLayouts.SetVisible("Success UI");
        }

        private void OnSolveFailure(Vector3Int position, Vector3Int direction)
        {
            m_state = State.ViewingResult;
            PlaceLeak(position, direction);
            m_uiLayouts.SetVisible("Failure UI");
        }

        private void PlaceLeak(Vector3Int position, Vector3Int direction)
        {
            Vector3 pos = position + (Vector3) direction * 0.5f;
            m_leakParticles.transform.position = pos;
            m_leakParticles.transform.rotation = Quaternion.LookRotation(direction);
            m_leakParticles.gameObject.SetActive(true);
        }

        public void NextLevel()
        {
            throw new NotImplementedException();
        }
    }
}
