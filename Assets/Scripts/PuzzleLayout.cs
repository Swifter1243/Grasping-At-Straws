﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SLC_GameJam_2025_1
{
    public class PuzzleLayout : MonoBehaviour
    {
        public Vector3Int m_dimensions;
        public PuzzlePiece[] m_unsafePieces;
        
        private PuzzlePiece[] m_internalPieces;
    
        public PuzzlePiece this[Vector3Int index] => m_internalPieces[Index3DTo1D(index)];

        private int Index3DTo1D(Vector3Int index)
        {
            int linearIndex = index.x + index.y * m_dimensions.x + index.z * m_dimensions.y;
            return linearIndex;
        }

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            m_internalPieces = InitializePieces(m_unsafePieces).ToArray();
        }

        private IEnumerable<PuzzlePiece> InitializePieces(PuzzlePiece[] pieces)
        {
            int intendedLength = m_dimensions.x * m_dimensions.y * m_dimensions.z;

            if (intendedLength != pieces.Length)
            {
                throw new Exception("Missing or excessive pieces when initializing PuzzleLayout.");
            }

            for (int x = 0; x < m_dimensions.x; x++)
            {
                for (int y = 0; y < m_dimensions.y; y++)
                {
                    for (int z = 0; z < m_dimensions.z; z++)
                    {
                        Vector3Int position = new(x, y, z);
                        PuzzlePiece piece = pieces.First(piece => piece.GetPiecePosition() == position);
                        yield return piece;
                    }
                }
            }
        }
    }   
}
