using UnityEngine;
namespace SLC_GameJam_2025_1
{
	public static class BoundsExtensions
	{
		public static float GetMaxAxis(this Bounds bounds)
		{
			return Mathf.Max(bounds.extents.x, Mathf.Max(bounds.extents.y, bounds.extents.z));;
		}
	}
}
