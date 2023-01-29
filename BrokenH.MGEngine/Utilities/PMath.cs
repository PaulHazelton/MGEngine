using Microsoft.Xna.Framework;

namespace BrokenH.MGEngine.Utilities
{
	public class PMath
	{
		#region Commented out stuff

		// // SIN TABLES	__________________________________________
		// private static float[] sinTable;
		// private static int sinTableResolution;

		// public static void initializeSinTable(int resolution)
		// {
		// 	sinTable = new float[resolution];
		// 	sinTableResolution = resolution;

		// 	for (int i = 0; i < resolution; i++)
		// 	{
		// 		sinTable[i] = (float)Math.Sin(map(i, 0, resolution, 0, (float)Math.PI * 2));
		// 	}
		// }
		// public static float getSinTableResolution()
		// { return PMath.sinTableResolution; }

		// public static float sinTableOf(float angle)
		// {
		// 	if (sinTable == null)
		// 		throw new InvalidOperationException("ERROR, SIN TABLE NOT INITIALIZED");

		// 	angle = normalizeAngle1(angle);

		// 	float indexF = map(angle, 0, (float)Math.PI * 2, 0, sinTableResolution);
		// 	indexF = indexF % sinTableResolution;

		// 	return sinTable[(int)Math.Floor(indexF)];
		// }
		// public static float sinTableAt(int index)
		// {
		// 	if (sinTable == null)
		// 		throw new InvalidOperationException("ERROR, SIN TABLE NOT INITIALIZED");
		// 	return sinTable[index];
		// }

		// // NUMBERS	______________________________________________

		// public static bool zequals(Vector2 a, Vector2 b)
		// {
		// 	return (PMath.zequals(a.X, b.Y) && PMath.zequals(a.Y, b.Y));
		// }

		// public static Vector2 rotateVec(Vector2 v, float angle)
		// {
		// 	Vector2 vec = new Vector2(v.X, v.Y);

		// 	vec = cartesianToPolar(vec);
		// 	vec.Y += angle;
		// 	vec = polarToCartesian(vec);
		// 	return vec;
		// }
		// public static Vector2 rotateVecAbout(Vector2 v, Vector2 pointOfRotation, float angle)
		// {
		// 	Vector2 resultant = new Vector2(v.X, v.Y);
		// 	resultant = resultant - (pointOfRotation);
		// 	resultant = rotateVec(resultant, angle);
		// 	resultant = resultant + (pointOfRotation);
		// 	return resultant;
		// }

		// public static Vector2 cartesianToPolar(Vector2 vector)
		// {
		// 	float r = (float)distance(Vector2.Zero, vector);
		// 	float angle = (float)Math.Atan2(vector.Y, vector.X);
		// 	angle = PMath.normalizeAngle1(angle);

		// 	return new Vector2(r, angle);
		// }
		// public static Vector2 cartesianToPolar(Vector2 vec1, Vector2 Vector2)
		// {
		// 	return cartesianToPolar(Vector2 - (vec1));
		// }

		// public static Vector2 polarToCartesian(float r, float a)
		// {
		// 	float x = (float)(r * Math.Cos(a));
		// 	float y = (float)(r * Math.Sin(a));

		// 	return new Vector2(x, y);
		// }
		// public static Vector2 polarToCartesian(Vector2 v)
		// {
		// 	return polarToCartesian(v.X, v.Y);
		// }

		#endregion

		public static float Map(float lowIn, float highIn, float lowOut, float highOut, float amount)
		{
			float range1 = highIn - lowIn;
			float x1 = amount - lowIn;

			x1 = x1 / range1;

			float range2 = highOut - lowOut;
			float x2 = x1 * range2;

			return x2 + lowOut;
		}

		public static int ArrayModulo(int i, int length)
		{
			if (i >= length)
				return i % length;
			if (i < 0)
				return i % length + length;
			else
				return i;
		}

		public static Vector2 Midpoint(Vector2 a, Vector2 b)
		{
			Vector2 mid = new Vector2((a.X + b.X) / 2, (a.Y + b.Y) / 2);
			return mid;
		}
	}
}