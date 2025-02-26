using UnityEngine;

namespace SLC_GameJam_2025_1
{
    public class PuzzleObject : MonoBehaviour
    {
        protected Vector3Int GetChildBoardDirection(Transform child)
        {
            Vector3 childLocalPosition = transform.parent == null
                ? child.localPosition
                : transform.parent.InverseTransformPoint(child.transform.position);
            Vector3 offset = childLocalPosition - transform.localPosition;
            Vector3Int outDirection = Vector3Int.RoundToInt(offset.normalized);
            return outDirection;
        }
        
        public Vector3Int BoardPosition => new()
        {
            x = Mathf.RoundToInt(transform.localPosition.x),
            y = Mathf.RoundToInt(transform.localPosition.y),
            z = Mathf.RoundToInt(transform.localPosition.z)
        };
    }
}
