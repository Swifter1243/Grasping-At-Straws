using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SLC_GameJam_2025_1
{
    public class Game : MonoBehaviour
    {
        public List<PuzzleLayout> m_puzzles = new();
        public int m_currentPuzzleIndex = 0;
        public Transform m_puzzleHolder;
        public GizmoHandler m_gizmoHandler;
        public ParticleSystem m_leakParticles;
        public CameraSmoothing m_cameraSmoothing;
        public ObjectVisibilityList m_uiLayouts;
        public SceneInteraction m_sceneInteraction;
        public float m_transitionInTime = 1.5f;
        public float m_transitionOutTime = 1.5f;
        public event Action<PuzzleLayout> onNewPuzzleStarted;
        public event Action<PuzzleLayout> onNewPuzzleLoaded;
        public event Action onPieceSelected;
        public event Action onLayerChanged;
        public GameObject m_winAnimation;
        public AudioSource m_sfxSource;
        public AudioClip m_hoverSound;
        public AudioClip m_selectSound;
        public AudioClip m_levelWinSound;
        public PauseMenu m_pauseMenu;

        private const float FOCUSED_OPACITY = 1;
        private const float UNFOCUSED_OPACITY = 0.2f;
        private const float DEFAULT_PLAYBACK_SPEED = 0.6f;


        public enum State
        {
            Transition,
            Editing,
            Solving,
            ViewingResult
        }

        public State m_state = State.Transition;
        private PuzzleLayout m_currentPuzzle;
        private float m_pipeProgress = 0;
        private PuzzlePiece m_selectedPiece = null;
        private PuzzlePiece m_hoveredPiece = null;
        private PuzzleSolution m_currentSolution = null;
        public Slider m_playbackSpeedSlider;
        private float m_playbackSpeed = DEFAULT_PLAYBACK_SPEED;
        private int m_selectedLayer = 0;
        private Bounds m_focusBounds;
        private float m_transitionTime = 0;

        private void Update()
        {
            if (m_state == State.Editing)
            {
                HandleLayerChangeInputs();
            }
        }

        private void Awake()
        {
            m_playbackSpeedSlider.value = m_playbackSpeed;
            m_playbackSpeedSlider.onValueChanged.AddListener(UpdatePlaybackSpeed);
            m_gizmoHandler.Close();
        }

        public void StartGame()
        {
            m_pauseMenu.Open();
            m_cameraSmoothing.enabled = true;
            NextLevel();
        }

        public void NextLevel()
        {
            if (m_currentPuzzle != null)
            {
                Destroy(m_currentPuzzle.gameObject);
                m_currentPuzzle = null;
            }

            if (m_currentPuzzleIndex >= m_puzzles.Count)
            {
                StartCoroutine(Win());
                return;
            }

            PlayerPrefs.SetInt("level", m_currentPuzzleIndex);
            PuzzleLayout puzzleToLoad = m_puzzles[m_currentPuzzleIndex++];
            onNewPuzzleLoaded?.Invoke(puzzleToLoad);
            m_currentPuzzle = Instantiate(puzzleToLoad, m_puzzleHolder);
            m_currentPuzzle.transform.localPosition = -m_currentPuzzle.Center;
            m_cameraSmoothing.SetFromBounds(m_currentPuzzle.BoundingBox);
            StartCoroutine(TransitionIn());
        }

        public void ExitLevel()
        {
            m_uiLayouts.SetVisible("Transition UI");
            StartCoroutine(TransitionOut());
        }

        private IEnumerator Win()
        {
            m_cameraSmoothing.m_acceptingInput = false;
            m_cameraSmoothing.m_targetPivot = Vector3.zero;
            m_cameraSmoothing.m_targetRotation = Quaternion.identity;
            m_cameraSmoothing.m_targetDistance = 12;
            m_cameraSmoothing.m_smoothRate = 6;

            while (!m_cameraSmoothing.ReachedDistanceTarget() && Mathf.Abs(m_cameraSmoothing.m_distanceFactor - 1) > 0.01f)
            {
                m_cameraSmoothing.m_distanceFactor = CameraSmoothing.LerpSmooth(1, m_cameraSmoothing.m_distanceFactor, Time.deltaTime, 20);
                yield return null;
            }
            m_cameraSmoothing.m_distanceFactor = 1;

            m_winAnimation.SetActive(true);
        }

        private IEnumerator TransitionIn()
        {
            m_transitionTime = 0;

            while (true)
            {
                m_transitionTime += Time.deltaTime / m_transitionInTime;

                float timeInOutCubic = Easing.InOutCubic(m_transitionTime);

                m_puzzleHolder.rotation = Quaternion.Slerp(Quaternion.Euler(30, 59, 39), Quaternion.identity, timeInOutCubic);
                m_puzzleHolder.position = Vector3.Lerp(new Vector3(0, -0.4f, 0), Vector3.zero, timeInOutCubic);
                m_puzzleHolder.localScale = Vector3.one * timeInOutCubic;

                if (m_transitionTime >= 1)
                {
                    m_puzzleHolder.position = Vector3.zero;
                    m_puzzleHolder.localScale = Vector3.one;
                    m_puzzleHolder.rotation = Quaternion.identity;

                    yield return new WaitForSeconds(0.5f);

                    StartPuzzleGameplay();
                    break;
                }

                yield return null;
            }
        }

        private IEnumerator TransitionOut()
        {
            m_transitionTime = 0;

            while (true)
            {
                m_transitionTime += Time.deltaTime / m_transitionOutTime;

                float timeInOutCubic = Easing.InOutCubic(m_transitionTime);

                m_puzzleHolder.rotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(30, 59, 39), timeInOutCubic);
                m_puzzleHolder.position = Vector3.Lerp(Vector3.zero, new Vector3(0, 0.4f, 0), timeInOutCubic);
                m_puzzleHolder.localScale = Vector3.one * (1 - timeInOutCubic);

                if (m_transitionTime >= 1)
                {
                    m_puzzleHolder.localScale = Vector3.zero;
                    NextLevel();
                    break;
                }

                yield return null;
            }
        }

        private void HandleLayerChangeInputs()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ChangeSelectedLayer(Math.Max(m_selectedLayer - 1, 0));
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                ChangeSelectedLayer(Math.Min(m_selectedLayer + 1, m_currentPuzzle.m_dimensions.y - 1));
            }
        }

        private void ChangeSelectedLayer(int layer)
        {
            if (layer == m_selectedLayer)
                return;

            onLayerChanged?.Invoke();

            m_selectedLayer = layer;
            UpdateLayerView();

            if (m_selectedPiece)
            {
                Vector3Int newPos = m_selectedPiece.BoardPosition;
                newPos.y = layer;

                PuzzlePiece old = m_selectedPiece;
                PuzzlePiece piece = m_currentPuzzle[newPos];
                if (piece)
                {
                    SelectPiece(piece);
                }
                else
                {
                    DeselectPiece();
                }

                old.SetInteractable(false);
            }
        }

        private void UpdateLayerView()
        {
            foreach (PuzzlePiece puzzlePiece in m_currentPuzzle)
            {
                bool onFloor = puzzlePiece.BoardPosition.y == m_selectedLayer;
                puzzlePiece.SetOpacity(onFloor ? FOCUSED_OPACITY : UNFOCUSED_OPACITY);
                puzzlePiece.SetInteractable(onFloor);
            }

            m_focusBounds = m_currentPuzzle.GetBoundingBoxAtLayer(m_selectedLayer);
            ResetView();
        }

        public void HoverPiece(PuzzlePiece piece)
        {
            if (m_hoveredPiece == piece)
            {
                return;
            }

            m_sfxSource.PlayOneShot(m_hoverSound);

            UnHoverPiece();
            m_hoveredPiece = piece;
            m_hoveredPiece.SetHovered(true);
        }

        public void UnHoverPiece()
        {
            if (m_hoveredPiece == null)
                return;

            m_hoveredPiece.SetHovered(false);
            m_hoveredPiece = null;
        }

        public void SelectPiece(PuzzlePiece piece)
        {
            DeselectPiece();
            piece.Select();
            m_sfxSource.PlayOneShot(m_selectSound);
            m_gizmoHandler.Open(piece);
            m_selectedPiece = piece;
            m_cameraSmoothing.m_targetPivot = piece.transform.position;
            m_cameraSmoothing.m_targetDistance = m_currentPuzzle.BoundingBox.GetMaxAxis() * 2;
            onPieceSelected?.Invoke();
        }

        public void DeselectPiece()
        {
            if (!m_selectedPiece)
                return;

            m_gizmoHandler.Close();
            ResetView();
            m_selectedPiece.Deselect();
            m_selectedPiece = null;
            UnHoverPiece();
        }

        public void StartEditing()
        {
            m_state = State.Editing;
            m_uiLayouts.Initialize();
            if (m_currentPuzzle.m_onBoardingType != OnBoardingType.BasicControls)
            {
                m_uiLayouts.SetVisible("Editor UI");
            }
            m_leakParticles.gameObject.SetActive(false);
            UpdateLayerView();
            foreach (PuzzlePiece puzzlePiece in m_currentPuzzle)
            {
                puzzlePiece.SetFluidProgress(0);
            }
        }

        private void StartPuzzleGameplay()
        {
            m_currentPuzzle.Initialize();
            m_gizmoHandler.Make3D(m_currentPuzzle.Is3D);
            m_selectedLayer = 0;
            onNewPuzzleStarted?.Invoke(m_currentPuzzle);

            if (m_currentPuzzle.m_onBoardingType == OnBoardingType.BasicControls)
            {
                m_sceneInteraction.m_interactionEnabled = false;
            }
            StartEditing();
            UpdateLayerView();
        }


        private void UpdatePlaybackSpeed(float playbackSpeed)
        {
            m_playbackSpeed = playbackSpeed;
        }

        private void ResetView()
        {
            m_cameraSmoothing.SetFromBounds(m_focusBounds);
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
                m_cameraSmoothing.SetFromBounds(m_currentPuzzle.BoundingBox);
                m_currentSolution = m_currentPuzzle.Solve();
                m_state = State.Solving;
                m_currentPuzzle.SetAllPipesOpacity(UNFOCUSED_OPACITY);
                m_playbackSpeedSlider.value = DEFAULT_PLAYBACK_SPEED;

                if (m_currentSolution.m_first == null)
                {
                    OnSolveFailure(m_currentPuzzle.m_in.BoardPosition, m_currentPuzzle.m_in.BoardDirection);
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
                m_pipeProgress += Time.deltaTime * m_playbackSpeed;
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
            m_sfxSource.PlayOneShot(m_levelWinSound);
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
            Vector3 pos = position + (Vector3) direction * 0.5f - m_currentPuzzle.Center;
            m_leakParticles.transform.position = pos;
            m_leakParticles.transform.rotation = Quaternion.LookRotation(direction);
            m_leakParticles.gameObject.SetActive(true);
        }
    }
}
