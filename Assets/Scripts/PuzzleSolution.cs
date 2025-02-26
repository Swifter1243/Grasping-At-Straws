using UnityEngine;

namespace SLC_GameJam_2025_1
{
    public class PuzzleSolution
    {
        public class Entry
        {
            public PuzzlePiece m_piece;
            public bool m_enteredInput1;
            public Vector3Int m_directionOut;
            public Entry m_next;
        }

        public Entry m_first;
        public Entry m_last;
        public bool m_success = false;
    }
}
