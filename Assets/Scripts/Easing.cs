using UnityEngine;
namespace SLC_GameJam_2025_1
{
	public static class Easing
	{
		public static float InOutCubic(float x)
		{
			return x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
		}

		public static float InCubic(float x)
		{
			return x * x * x;
		}

		public static float OutCubic(float x)
		{
			return 1 - Mathf.Pow(1 - x, 3);
		}
	}
}
