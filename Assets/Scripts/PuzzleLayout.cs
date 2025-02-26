using UnityEngine;

namespace SLC_GameJam_2025_1
{
    public class PuzzleLayout
    {
        private Vector3Int m_dimensions;
        private PuzzlePiece[] m_pieces;
    
        public PuzzlePiece this[Vector3Int index] => m_pieces[Index3DTo1D(index)];

        private int Index3DTo1D(Vector3Int index)
        {
            int linearIndex = index.x + index.y * m_dimensions.x + index.z * m_dimensions.y;
            return linearIndex;
        }

        public void Initialize(Vector3Int dimensions, PuzzlePiece[] pieces)
        {
            InitializePieces(pieces);
        }

        private void InitializePieces(PuzzlePiece[] pieces)
        {
            // TODO
        }
    }   
}
