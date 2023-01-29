using Microsoft.Xna.Framework;

namespace BrokenH.MGEngine.Extensions
{
	public static class VectorExtensions
	{
		public static Vector2 Round(this Vector2 v, int multiple)
		{
			// Offset to round instead of floor
			float x = v.X + (multiple / 2f);
			float y = v.Y + (multiple / 2f);

			int xx = ((int)(x / multiple)) * multiple;
			int yy = ((int)(y / multiple)) * multiple;

			return new Vector2(xx, yy);
		}

		public static float Angle(this Vector2 v)
		{
			return (float)System.Math.Atan2(v.Y, v.X);
		}

		public static bool IsOrthogonal(this Vector2 v, float tolerance = 0.1f)
		{
			v.Normalize();
			tolerance = 1 - tolerance;

			var r = new Vector2( 1,  0);
			var d = new Vector2( 0,  1);
			var l = new Vector2(-1,  0);
			var u = new Vector2( 0, -1);

			return (Vector2.Dot(v, r) >= tolerance
				 || Vector2.Dot(v, d) >= tolerance
				 || Vector2.Dot(v, l) >= tolerance
				 || Vector2.Dot(v, u) >= tolerance);
		}

		public static Vector2 Flip(this Vector2 v)
		{
			return new Vector2(-v.X, -v.Y);
		}
	}
}