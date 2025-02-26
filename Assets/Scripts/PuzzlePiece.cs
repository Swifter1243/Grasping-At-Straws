using System;
using UnityEngine;

namespace SLC_GameJam_2025_1
{
    public class PuzzlePiece : MonoBehaviour
    {
        public Transform m_input1;
        public Transform m_input2;

        private readonly static Color s_input1Color = new(0, 0.5f, 0.4f);
        private readonly static Color s_input2Color = new(1, 0.4f, 0);

        public Vector3Int GetPiecePosition() => new()
        {
            x = Mathf.RoundToInt(transform.localPosition.x),
            y = Mathf.RoundToInt(transform.localPosition.y),
            z = Mathf.RoundToInt(transform.localPosition.z)
        };

        private void OnDrawGizmos()
        {
            Debug.DrawLine(transform.position, transform.position + Input1Direction, s_input1Color);
            Debug.DrawLine(transform.position, transform.position + Input2Direction, s_input2Color);
        }

        public Vector3Int Input1Direction => GetChildDirection(m_input1);
        public Vector3Int Input2Direction => GetChildDirection(m_input2);

        private Vector3Int GetChildDirection(Transform child)
        {
            Vector3 childLocalPosition = transform.parent == null
                ? child.localPosition
                : transform.parent.InverseTransformPoint(child.transform.position);
            Vector3 offset = childLocalPosition - transform.localPosition;
            Vector3Int outDirection = Vector3Int.RoundToInt(offset.normalized);
            return outDirection;
        }
    }
}
