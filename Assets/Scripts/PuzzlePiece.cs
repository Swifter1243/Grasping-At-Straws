using System;
using UnityEngine;

namespace SLC_GameJam_2025_1
{
    public class PuzzlePiece : MonoBehaviour
    {
        public Transform m_in;
        public Transform m_out;

        private readonly static Color s_inColor = new(0, 0.5f, 0.4f);
        private readonly static Color s_outColor = new(1, 0.4f, 0);

        public Vector3Int GetPiecePosition() => new()
        {
            x = Mathf.RoundToInt(transform.localPosition.x),
            y = Mathf.RoundToInt(transform.localPosition.y),
            z = Mathf.RoundToInt(transform.localPosition.z)
        };

        private void OnDrawGizmos()
        {
            Debug.DrawLine(transform.position, transform.position + InDirection, s_inColor);
            Debug.DrawLine(transform.position, transform.position + OutDirection, s_outColor);
        }

        public Vector3Int InDirection => GetChildDirection(m_in);
        public Vector3Int OutDirection => GetChildDirection(m_out);

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
