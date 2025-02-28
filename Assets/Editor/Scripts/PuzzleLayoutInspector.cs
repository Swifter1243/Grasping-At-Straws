using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace SLC_GameJam_2025_1
{
	[CustomEditor(typeof(PuzzleLayout))]
	public class PuzzleLayoutInspector : Editor
	{
		private static readonly Random s_mRand = new();

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			PuzzleLayout layout = (PuzzleLayout) target;

			if (GUILayout.Button("Randomize Piece Directions"))
			{
				RandomizeAllPieceDirections(layout);
			}
			if (GUILayout.Button("Center All Pieces"))
			{
				CenterAllPieces(layout);
			}
		}

		private void MarkSceneDirty()
		{
			EditorUtility.SetDirty(this);
			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(target.GameObject().scene);
		}

		private void CenterAllPieces(PuzzleLayout layout)
		{
			foreach (PuzzlePiece puzzlePiece in layout.GetComponentsInChildren<PuzzlePiece>())
			{
				puzzlePiece.transform.localPosition = Vector3Int.RoundToInt(puzzlePiece.transform.localPosition);
			}

			MarkSceneDirty();
		}

		private float GetRandom90DegreeAngle()
		{
			return s_mRand.Next() % 4 * 90;
		}
		private void RandomizeAllPieceDirections(PuzzleLayout layout)
		{

			foreach (PuzzlePiece puzzlePiece in layout.GetComponentsInChildren<PuzzlePiece>())
			{
				puzzlePiece.transform.rotation = layout.Is3D ?
					Quaternion.Euler(GetRandom90DegreeAngle(), GetRandom90DegreeAngle(), GetRandom90DegreeAngle()) :
					Quaternion.Euler(0, GetRandom90DegreeAngle(), 0);
			}

			MarkSceneDirty();
		}
	}
}
