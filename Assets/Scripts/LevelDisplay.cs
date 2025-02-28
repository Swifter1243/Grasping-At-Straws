using System.Collections;
using TMPro;
using UnityEngine;

namespace SLC_GameJam_2025_1
{
    public class LevelDisplay : Fadeable
    {
        public float m_stallDuration = 0.5f;
        public float m_fadeInDuration = 1f;
        public float m_fadeOutDuration = 1f;
        public TextMeshProUGUI m_levelNumber;
        public TextMeshProUGUI m_levelText;
        public Game m_game;

        protected override void Awake()
        {
            base.Awake();
            m_game.onNewPuzzleLoaded += LoadLevel;
            m_game.onNewPuzzleStarted += _ => StartCoroutine(Fade(m_fadeOutDuration, 1, 0));
        }

        private void LoadLevel(PuzzleLayout puzzleLayout)
        {
            m_levelNumber.text = $"Level {m_game.m_currentPuzzleIndex}";
            m_levelText.text = puzzleLayout.name;
            StartCoroutine(TransitionIn());
        }

        private IEnumerator TransitionIn()
        {
            yield return new WaitForSeconds(m_stallDuration);
            yield return Fade(m_fadeInDuration, 0, 1);
        }
    }
}
