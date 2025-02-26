using UnityEngine;

namespace SLC_GameJam_2025_1
{
    public class PuzzleInput : PuzzleObject
    {
        public Transform m_directionTransform;
        
        private void OnDrawGizmos()
        {
            Debug.DrawLine(transform.position, transform.position + BoardDirection, Color.blue);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, Vector3.one);
        }
        
        public Vector3Int BoardDirection => GetChildBoardDirection(m_directionTransform);
    }
}
