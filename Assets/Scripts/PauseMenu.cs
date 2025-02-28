using UnityEngine;
namespace SLC_GameJam_2025_1
{
	public class PauseMenu : Fadeable
	{
		public SceneInteraction m_sceneInteraction;
		public GameObject m_pauseWindow;
		public bool m_paused = false;

		public void Open()
		{
			StartCoroutine(Fade(1, 0, 1));
		}

		public void ToggleWindowOpen()
		{
			m_paused = !m_paused;
			m_pauseWindow.SetActive(m_paused);
			m_sceneInteraction.m_scenePaused = m_paused;
		}
	}
}
