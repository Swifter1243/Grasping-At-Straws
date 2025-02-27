using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SLC_GameJam_2025_1
{
    public class PuzzleLayout : MonoBehaviour, IEnumerable<PuzzlePiece>
    {
        // Inspector
        public Vector3Int m_dimensions;
        public PuzzleInput m_in;
        public PuzzleInput m_out;

        // Parsed
        private Dictionary<Vector3Int, PuzzlePiece> m_internalPieces;

        private PuzzlePiece this[Vector3Int index] => m_internalPieces.ContainsKey(index) ? m_internalPieces[index] : null;

        public void Initialize()
        {
            PuzzlePiece[] unsafePieces = GetComponentsInChildren<PuzzlePiece>();
            m_internalPieces = InitializePieces(unsafePieces).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public void SetAllPipesOpacity(float opacity)
        {
            foreach (PuzzlePiece puzzlePiece in this)
            {
                puzzlePiece.SetOpacity(opacity);
            }
        }

        private int Volume => m_dimensions.x * m_dimensions.y * m_dimensions.z;
        public Bounds BoundingBox => new(Vector3.zero, m_dimensions);
        public Vector3 Center => (m_dimensions - Vector3.one) / 2;

        public Bounds GetBoundingBoxAtLayer(int layer)
        {
            return new Bounds(new Vector3(0, layer - Center.y, 0), new Vector3(m_dimensions.x, 1, m_dimensions.z));
        }

        private IEnumerable<KeyValuePair<Vector3Int, PuzzlePiece>> InitializePieces(PuzzlePiece[] pieces)
        {
            for (int x = 0; x < m_dimensions.x; x++)
            {
                for (int y = 0; y < m_dimensions.y; y++)
                {
                    for (int z = 0; z < m_dimensions.z; z++)
                    {
                        Vector3Int position = new(x, y, z);
                        PuzzlePiece piece = pieces.FirstOrDefault(piece => piece.BoardPosition == position);

                        if (!piece)
                            continue;

                        yield return new KeyValuePair<Vector3Int, PuzzlePiece>(position, piece);
                    }
                }
            }
        }

        public PuzzleSolution Solve()
        {
            PuzzleSolution solution = new();

            Vector3Int searchPoint = m_in.BoardPosition;
            Vector3Int searchDirection = m_in.BoardDirection;
            PuzzleSolution.Entry last = new();

            int maxIterations = Volume + 2;
            for (int i = 0; i < maxIterations; i++)
            {
                PuzzleObject puzzleObject = GetObjectFromDirection(searchPoint, searchDirection);

                if (puzzleObject == null)
                {
                    solution.m_success = false;
                    break;
                }

                if (puzzleObject is PuzzleInput)
                {
                    solution.m_success = true;
                    break;
                }

                if (puzzleObject is PuzzlePiece puzzlePiece)
                {
                    searchDirection = puzzlePiece.GetDirectionOut(searchDirection, out bool enteredInput1);
                    searchPoint = puzzlePiece.BoardPosition;

                    PuzzleSolution.Entry entry = new()
                    {
                        m_piece = puzzlePiece,
                        m_enteredInput1 = enteredInput1,
                        m_directionOut = searchDirection,
                    };
                    last.m_next = entry;
                    last = entry;

                    solution.m_first ??= entry;
                    solution.m_last = entry;
                }

                if (i == maxIterations - 1)
                {
                    Debug.LogError("Solution was never solved. Something went wrong.");
                }
            }

            return solution;
        }

        private PuzzleObject GetObjectFromDirection(Vector3Int position, Vector3Int direction)
        {
            Vector3Int nextPosition = position + direction;

            bool outOfBoundsX = nextPosition.x < 0 || nextPosition.x >= m_dimensions.x;
            bool outOfBoundsY = nextPosition.y < 0 || nextPosition.y >= m_dimensions.y;
            bool outOfBoundsZ = nextPosition.z < 0 || nextPosition.z >= m_dimensions.z;
            bool outOfBounds = outOfBoundsX || outOfBoundsY || outOfBoundsZ;

            if (outOfBounds)
            {
                return nextPosition == m_out.BoardPosition ? m_out : null;
            }

            PuzzlePiece piece = this[nextPosition];

            if (piece == null || !piece.AcceptsDirection(direction))
            {
                return nextPosition == m_out.BoardPosition ? m_out : null;
            }

            return piece;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(Center, m_dimensions);
        }

        public IEnumerator<PuzzlePiece> GetEnumerator() => m_internalPieces.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
