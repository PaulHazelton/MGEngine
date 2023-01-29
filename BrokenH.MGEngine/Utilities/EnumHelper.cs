using System;

namespace BrokenH.MGEngine.Utilities
{
	public static class EnumHelpers
	{
		public static T[] GetValues<T>()
		{
			return (T[])Enum.GetValues(typeof(T));
		}
	}
}