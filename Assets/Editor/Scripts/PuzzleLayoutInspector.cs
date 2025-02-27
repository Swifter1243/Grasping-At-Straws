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

			PuzzleLayout layout = (PuzzleLayout)target;

			if (GUILayout.Button("Randomize Piece Directions"))
			{
				RandomizeAllPieceDirections(layout);
			}
		}

		private float GetRandom90DegreeAngle()
		{
			return s_mRand.Next() % 4 * 90;
		}
		public void RandomizeAllPieceDirections(PuzzleLayout layout)
		{
			foreach (PuzzlePiece puzzlePiece in layout.GetComponentsInChildren<PuzzlePiece>())
			{
				puzzlePiece.transform.rotation = Quaternion.Euler(GetRandom90DegreeAngle(), GetRandom90DegreeAngle(), GetRandom90DegreeAngle());
			}

			EditorUtility.SetDirty(this);
			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(target.GameObject().scene);
		}
	}
}
