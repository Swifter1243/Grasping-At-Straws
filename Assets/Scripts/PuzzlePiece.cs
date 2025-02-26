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
            Debug.DrawLine(transform.position, transform.position + Input1DirectionOut, s_input1Color);
            Debug.DrawLine(transform.position, transform.position + Input2DirectionOut, s_input2Color);
        }

        public Vector3Int Input1DirectionOut => GetChildBoardDirection(m_input1);
        public Vector3Int Input1DirectionIn => GetChildBoardDirection(m_input1) * -1;
        public Vector3Int Input2DirectionOut => GetChildBoardDirection(m_input2);
        public Vector3Int Input2DirectionIn => GetChildBoardDirection(m_input2) * -1;
        
        public bool AcceptsDirection(Vector3Int direction) => direction == Input1DirectionIn || direction == Input2DirectionIn;
        
        public Vector3Int GetDirectionOut(Vector3Int directionIn, out bool usingInput1)
        {
            if (!AcceptsDirection(directionIn))
            {
                throw new ArgumentException("Invalid direction");
            }

            usingInput1 = directionIn == Input1DirectionIn;
            return usingInput1 ? Input2DirectionOut : Input1DirectionOut;
        }
    }
}
