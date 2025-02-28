using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace SLC_GameJam_2025_1
{
	public class MainMenu : Fadeable
	{
		public Game m_game;
		public CameraSmoothing m_cameraSmoothing;
		public Button m_playButton;
		public Button m_continueButton;
		public Button m_restartButton;
		public Button m_exitButton;

		private void StartGame()
		{
			StartCoroutine(Fade(0.5f, 1, 0));
			m_canvasGroup.interactable = false;
			m_cameraSmoothing.enabled = true;
			m_game.NextLevel();
		}

		protected override void Awake()
		{
			base.Awake();

			bool saveExists = PlayerPrefs.HasKey("level");

			m_exitButton.gameObject.SetActive(true);

			if (saveExists)
			{
				m_continueButton.gameObject.SetActive(true);
				m_restartButton.gameObject.SetActive(true);
			}
			else
			{
				m_playButton.gameObject.SetActive(true);
			}

			StartCoroutine(Fade(0.5f, 0, 1));
		}

		public void ExitGame()
		{
			#if UNITY_EDITOR
			EditorApplication.ExitPlaymode();
			#else
			Application.Quit();
			#endif
		}

		public void PlayGame()
		{
			StartGame();
		}

		public void ContinueGame()
		{
			int level = PlayerPrefs.GetInt("level");
			m_game.m_currentPuzzleIndex = level;
			StartGame();
		}

		public void RestartGame()
		{
			PlayerPrefs.SetInt("level", 0);
			StartGame();
		}
	}
}
