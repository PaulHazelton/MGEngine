using System;
using Microsoft.Xna.Framework;

namespace BrokenH.MGEngine.Utilities
{
	public enum Direction
	{
		Right,
		Down,
		Left,
		Up
	}

	public static class DirectionExtensions
	{
		private const float Right = 0f;
		private const float Down = (float)(Math.PI * 0.5d);
		private const float Left = (float)(Math.PI * 1.0d);
		private const float Up = (float)(Math.PI * 1.5d);

		public static Direction Flip(this Direction d) => d switch
		{
			Direction.Right => Direction.Left,
			Direction.Down => Direction.Up,
			Direction.Left => Direction.Right,
			Direction.Up => Direction.Down,
			_ => throw new System.NotImplementedException()
		};

		public static Direction GetDirection(this Vector2 v)
		{
			float angle = (float)System.Math.Atan2(v.Y, v.X);
			float br = (float)( 1 * Math.PI / 4);
			float bl = (float)( 3 * Math.PI / 4);
			float tr = (float)(-1 * Math.PI / 4);
			float tl = (float)(-3 * Math.PI / 4);

			if (angle >= br && angle < bl)
				return Direction.Down;
			else if (angle <= tr && angle > tl)
				return Direction.Up;
			else if (angle <= tl || angle >= bl)
				return Direction.Left;
			else
				return Direction.Right;
		}

		public static float Angle(this Direction d) => d switch
		{
			Direction.Right => Right,
			Direction.Down => Down,
			Direction.Left => Left,
			Direction.Up => Up,
			_ => 0f
		};

		public static Vector2 Vector(this Direction d, float magnitude) => d switch
		{
			Direction.Right => new Vector2(magnitude, 0),
			Direction.Down => new Vector2(0, magnitude),
			Direction.Left => new Vector2(-magnitude, 0),
			Direction.Up => new Vector2(0, -magnitude),
			_ => throw new NotImplementedException("Direction." + d + " not implimented"),
		};

		public static bool IsHorizontal(this Direction d) => (int)d % 2 == 0;
		public static bool IsVertical(this Direction d) => (int)d % 2 == 1;

		/// <summary>
		/// <list>
		/// 	<item>0 = Right</item>
		/// 	<item>1 = Down</item>
		/// 	<item>2 = Left</item>
		/// 	<item>3 = Up</item>
		/// </list>
		/// </summary>
		public static Direction get(int i)
		{
			i = i % 4;
			switch (i)
			{
				case 0: return Direction.Right;
				case 1: return Direction.Down;
				case 2: return Direction.Left;
				case 3: return Direction.Up;
				default: return get(4 + i);
			}
		}

		public static Direction RotateCW(this Direction d) => d switch
		{
			Direction.Right => Direction.Down,
			Direction.Down => Direction.Left,
			Direction.Left => Direction.Up,
			Direction.Up => Direction.Right,
			_ => Direction.Right,
		};
		public static Direction RotateCCW(this Direction d) => d switch
		{
			Direction.Right => Direction.Up,
			Direction.Down => Direction.Right,
			Direction.Left => Direction.Down,
			Direction.Up => Direction.Left,
			_ => Direction.Right,
		};
	}
}