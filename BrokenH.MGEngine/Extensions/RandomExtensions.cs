using System;

namespace BrokenH.MGEngine.Extensions
{
	public static class RandomExtensions
	{
		public static bool NextBool(this Random r)
		{
			return r.NextSingle() < 0.5f;
		}

		/// <summary>
		/// Returns a random float that is within a specified range.
		/// </summary>
		/// <param name="min">The inclusive lower bound of the random number returned.</param>
		/// <param name="max">The exclusive upper bound of the random number returned.
		/// max must be greater than or equal to min.</param>
		/// <returns></returns>
		public static float NextSingle(this Random r, float min, float max)
		{
			return min + r.NextSingle() * (max - min);
		}
	}
}