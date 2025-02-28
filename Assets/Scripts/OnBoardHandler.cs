using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SLC_GameJam_2025_1
{
    public class OnBoardHandler : MonoBehaviour
    {
        public Game m_game;
        public GizmoHandler m_gizmoHandler;
        public CameraSmoothing m_cameraSmoothing;
        public ObjectVisibilityList m_uiLayouts;
        public SceneInteraction m_sceneInteraction;
        public OnBoardingUI m_panning;
        public OnBoardingUI m_scrolling;
        public OnBoardingUI m_selectPieces;
        public OnBoardingUI m_rotatePieces;
        public OnBoardingUI m_layers;

        private void Awake()
        {
            m_game.onNewPuzzleStarted += OnNewPuzzleLoaded;
        }

        private void SetupPanning()
        {
            UnityEvent onComplete = m_panning.Setup();
            m_cameraSmoothing.onRotationChanged += () => onComplete.Invoke();
            m_panning.onFinished += SetupScrolling;
        }

        private void SetupScrolling()
        {
            UnityEvent onComplete = m_scrolling.Setup();
            m_cameraSmoothing.onDistanceChanged += () => onComplete.Invoke();
            m_scrolling.onFinished += SetupSelectPieces;
        }

        private void SetupSelectPieces()
        {
            m_sceneInteraction.m_interactionEnabled = true;
            UnityEvent onComplete = m_selectPieces.Setup();
            m_game.onPieceSelected += () => onComplete.Invoke();
            m_selectPieces.onFinished += SetupRotatePieces;
        }

        private void SetupRotatePieces()
        {
            UnityEvent onComplete = m_rotatePieces.Setup();
            m_gizmoHandler.onGizmoUsed += () => onComplete.Invoke();
            m_rotatePieces.onFinished += () => m_uiLayouts.SetVisible("Editor UI");
        }

        private void SetupLayers()
        {
            UnityEvent onComplete = m_layers.Setup();
            m_game.onLayerChanged += () => onComplete.Invoke();
        }

        private void OnNewPuzzleLoaded(PuzzleLayout puzzleLayout)
        {
            switch (puzzleLayout.m_onBoardingType)
            {
            case OnBoardingType.None:
                break;
            case OnBoardingType.BasicControls:
                SetupPanning();
                break;
            case OnBoardingType.Start3D:
                SetupLayers();
                break;
            }
        }
    }
}
