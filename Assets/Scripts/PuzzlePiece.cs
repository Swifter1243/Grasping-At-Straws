using UnityEngine;

namespace SLC_GameJam_2025_1
{
    public class PuzzlePiece : MonoBehaviour
    {
        public Vector3Int GetPiecePosition() => new()
        {
            x = Mathf.RoundToInt(transform.localPosition.x),
            y = Mathf.RoundToInt(transform.localPosition.y),
            z = Mathf.RoundToInt(transform.localPosition.z)
        };
    }
}
