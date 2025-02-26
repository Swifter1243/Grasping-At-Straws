using System;
using UnityEngine;

namespace SLC_GameJam_2025_1
{
    public class Level : MonoBehaviour
    {
        public Vector3Int m_layoutDimensions;
        public PuzzlePiece[] m_puzzlePieces;
        private readonly PuzzleLayout m_puzzleLayout = new();

        private void Awake()
        {
            m_puzzleLayout.Initialize(m_layoutDimensions, m_puzzlePieces);
        }
    }
}
