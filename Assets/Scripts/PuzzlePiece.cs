using System;
using UnityEngine;

namespace SLC_GameJam_2025_1
{
    public class PuzzlePiece : PuzzleObject
    {
        public Transform m_input1;
        public Transform m_input2;

        private readonly static Color s_input1Color = new(0, 0.5f, 0.4f);
        private readonly static Color s_input2Color = new(1, 0.4f, 0);

        private void OnDrawGizmos()
        {
            Debug.DrawLine(transform.position, transform.position + Input1Direction, s_input1Color);
            Debug.DrawLine(transform.position, transform.position + Input2Direction, s_input2Color);
        }

        public Vector3Int Input1Direction => GetChildBoardDirection(m_input1);
        public Vector3Int Input2Direction => GetChildBoardDirection(m_input2);

        public bool AcceptsDirection(Vector3Int direction) => direction == Input1Direction || direction == Input2Direction;
    }
}
