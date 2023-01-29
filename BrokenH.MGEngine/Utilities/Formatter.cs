/*	AS OF 2018-07-02:
 *
 * 	Box2D density of 1 = 1000 kg/m^3 = 1 g/cm^3 = 1 g/mL
 * 	Box2D density:	g/cm^3
 */

using System;
using Microsoft.Xna.Framework;

namespace BrokenH.MGEngine.Utilities
{
	public static class Formatter
	{
		private const float MassScale = 1000;
		private const float DensityScale = 1000;
		private const float ForceScale = 1000;
		private static readonly string[,] prefixes = new string[,]
		{
			{"a" , "f" , "p" , "n" , "μ" , "m" , " " , "k" , "M", "G" , "T" , "P" , "E" },	//regular
			{"ag", "fg", "pg", "ng", "μg", "mg", " g", "kg", " t", "kt", "Mt", "Pg", "Eg"},	//mass
			{"a" , "f" , "p" , "n" , "μ" , "m" , " " , "K", "M", "G", "T", "P", "E"}		//no unit
		};
		private const int PrefixOffset = 6;

		public const double Epsilon = (1.0d / 100000.0d);
		public static int SigFigs = 4;

		public static string UnitToString(double value, string unit)
		{
			//Variables
			string text = "";   //text to return
			double newValue = 0;    //used for doing math while not changing value
			int mod3 = 0;   //num of digits mod 3 (sorta). (25 km -> mod3 = 2)
			int index1 = 0; //index for prefix. (25 km -> index1 = 1)
			int index2 = 0; //used for second normalization
			int intVal = 0; //integer value of adjusted number (25.35 km -> intVal = 25)
			int sigDigs = 0;    //the significant digits of the value. (25.35 km -> sigDigs = 2535)

			newValue = Math.Abs(value);

			//zero skip
			if (Math.Abs(newValue) < Epsilon)
			{
				if (unit.Length == 2)
					return " 0.000 " + unit;
				else
					return " 0.000  " + unit;
			}

			//Get the Units.sigFigs most significant digits of the new Value. This rounds it
			sigDigs = GetSigFigs(newValue, Formatter.SigFigs);

			//Normalize
			index1 = (int)Math.Floor(Math.Log10(newValue) / 3);
			newValue = newValue / Math.Pow(10, index1 * 3);
			//Round number
			intVal = RoundDoubleToInt(newValue * Math.Pow(10, Formatter.SigFigs - 1));
			intVal /= (int)Math.Pow(10, Formatter.SigFigs - 1);
			//number of digits mod 3 (sorta)
			mod3 = NumOfDigits(intVal);
			//normalize again (may have rolled over from 999.9 to 1.000 k)
			index2 = (int)Math.Floor(Math.Log10(intVal) / 3);
			intVal = (int)(intVal / Math.Pow(10, index2 * 3));
			//did it roll over?
			if (mod3 > NumOfDigits(intVal))
				index1 += 1;
			//calc mod3 again
			mod3 = NumOfDigits(intVal);


			//Building string
			string sigDigs_s = sigDigs.ToString();

			//positive or negative
			if (value > 0)
				text = " ";
			else
				text = "-";

			//Piece together 2 halves of string
			text += sigDigs_s.Substring(0, mod3);

			if (Formatter.SigFigs - mod3 > 0)
				text += "." + sigDigs_s.Substring(mod3);
			else
				text += ' ';

			text += ' ';

			//Units at end
			index1 += PrefixOffset;
			int unitType = 0;

			//kg exception
			if (unit == "kg")
			{
				index1 += 1;
				unitType = 1;
				unit = "";
			}
			else if (unit == "" || unit == null)
			{
				unitType = 2;
				unit = "";
			}

			//prefix of scientific notation if out of bounds
			if (index1 >= 0 && index1 < Formatter.prefixes.GetLength(1))
				unit = Formatter.prefixes[unitType, index1] + unit;
			else
				unit = "*10^" + (index1 * 3).ToString() + unit;

			//return number with unit
			text += unit;
			return text;
		}
		public static string UnitToStringSN(double value, string unit)
		{
			string text = "";
			string format = "";

			double newValue = Math.Abs(value);
			int numMod3 = 0;
			int index = 0;

			if (Math.Abs(value) < Epsilon)
				return "0.000 " + unit;

			index = (int)Math.Floor(Math.Log10(newValue) / 3);

			newValue = value / Math.Pow(10, index * 3);
			numMod3 = NumOfDigits(newValue);


			format = "%";
			format += (numMod3 + 1).ToString();
			format += ".";
			format += (4 - numMod3).ToString();
			format += "f";

			text = String.Format(format, newValue);
			text += unit;

			if (index != 0)
			{
				text += " *10^";
				text += (index * 3).ToString();
			}

			return text;
		}

		public static string VectorUnitToString(Vector2 v, string unit)
		{
			string text = Formatter.UnitToString(v.X, unit);
			text += ", ";
			text += Formatter.UnitToString(v.Y, unit);

			return text;
		}

		private static int NumOfDigits(double value)
		{
			value = Math.Abs(value);

			if (value == 0)
				return 1;

			int num = (int)(Math.Log10(value));
			num += (value > 1) ? 1 : -1;

			return num;
		}

		private static int NumOfDigits(int value)
		{
			return (int)(Math.Log10(value) + 1);
		}

		private static int GetSigFigs(double value, int digits)
		{
			//digits > 0

			//value = 0.9999999

			//normalize (make the num satisfy (1 >= x < 10))
			double normal = NormalizeDouble(value);
			//normal = 9.9999999

			normal *= Math.Pow(10, digits - 1);
			//normal = 9999.999

			int intVal = RoundDoubleToInt(normal);
			//intVal = 10000

			if (NumOfDigits(intVal) > digits)
			{
				intVal /= 10;
				//			System.out.println("tripped!");
			}
			//intVal = 1000
			return intVal;
		}

		private static double NormalizeDouble(double value)
		{
			double log10 = Math.Log10(value);
			value /= Math.Pow(10, Math.Floor(log10));

			return value;
		}

		private static int RoundDoubleToInt(double value)
		{
			return (int)(value + 0.5d);
		}
	}
}