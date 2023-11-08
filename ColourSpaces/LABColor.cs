using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Utilities; // FastMathf provides an optional method to approximate power calculations, which is used here to speed up colour rotations

namespace ColorSpaces
{
	// This adds the CIELAB color space to Unity's built-in RGB colour space. See https://en.wikipedia.org/wiki/CIELAB_color_space.
	// CIELAB is a color-opponent space with L for lightness and a and b for the color-opponent dimensions.
	// CIELAB approximates human vision and aspires to perceptual uniformity, with the L component closely matching human perception of lightness.
	[Serializable]
	public readonly struct LABColor : IEquatable<LABColor>
	{
		// lightness l and color opponent dimensions a and b
		public readonly float l, a, b;

		public LABColor(float l, float a, float b)
		{
			this.l = l;
			this.a = a;
			this.b = b;
		}

		public LABColor(Color col)
		{
			LABColor temp = FromColor(col);
			this.l = temp.l;
			this.a = temp.a;
			this.b = temp.b;
		}

		// converts color to LABColor
		public static LABColor FromColor(Color col)
        {
			float r = (col.r > 0.04045f) 
				? Mathf.Pow((col.r + 0.055f) * 0.94786729857f, 2.2f) 
				: (col.r * 0.077399380505084991455078125f);
			float g = (col.g > 0.04045f) 
				? Mathf.Pow((col.g + 0.055f) * 0.94786729857f, 2.2f) 
				: (col.g * 0.077399380505084991455078125f);
			float b = (col.b > 0.04045f) 
				? Mathf.Pow((col.b + 0.055f) * 0.94786729857f, 2.2f) 
				: (col.b * 0.077399380505084991455078125f);

			// convert rgb to intermediate xyz col space
			float x = (r * 0.4124f + g * 0.3576f + b * 0.1805f);
			float y = (r * 0.2126f + g * 0.7152f + b * 0.0722f);
			float z = (r * 0.0193f + g * 0.1192f + b * 0.9505f);
			x = (x > 0.9505f) 
				? 0.9505f 
				: ((x < 0f) ? 0f : x);
			y = (y > 1.0f) 
				? 1.0f 
				: ((y < 0f) ? 0f : y);
			z = (z > 1.089f) 
				? 1.089f 
				: ((z < 0f) ? 0f : z);

			float fx = x * 1.05207785376f;
			float fy = y;
			float fz = z * 0.91827364554f;
			fx = (fx > 0.008856f) 
				? Mathf.Pow(fx, 0.3333333432674407958984375f) 
				: (7.787f * fx + 0.13793103396892547607421875f);
			fy = (fy > 0.008856f) 
				? Mathf.Pow(fy, 0.3333333432674407958984375f) 
				: (7.787f * fy + 0.13793103396892547607421875f);
			fz = (fz > 0.008856f) 
				? Mathf.Pow(fz, 0.3333333432674407958984375f) 
				: (7.787f * fz + 0.13793103396892547607421875f);

			return new LABColor(
				l: (116.0f * fy) - 16f,
				a: 500.0f * (fx - fy),
				b: 200.0f * (fy - fz));
		}

		// converts color to LABColor
		public static LABColor FromColor_Fast(Color col)
		{
			float r = (col.r > 0.04045f)
				? FastMathf.Pow((col.r + 0.055f) * 0.94786729857f, 2.2f)
				: (col.r * 0.077399380505084991455078125f);
			float g = (col.g > 0.04045f)
				? FastMathf.Pow((col.g + 0.055f) * 0.94786729857f, 2.2f)
				: (col.g * 0.077399380505084991455078125f);
			float b = (col.b > 0.04045f)
				? FastMathf.Pow((col.b + 0.055f) * 0.94786729857f, 2.2f)
				: (col.b * 0.077399380505084991455078125f);

			// convert rgb to intermediate xyz col space
			float x = (r * 0.4124f + g * 0.3576f + b * 0.1805f);
			float y = (r * 0.2126f + g * 0.7152f + b * 0.0722f);
			float z = (r * 0.0193f + g * 0.1192f + b * 0.9505f);
			x = (x > 0.9505f)
				? 0.9505f
				: ((x < 0f) ? 0f : x);
			y = (y > 1.0f)
				? 1.0f
				: ((y < 0f) ? 0f : y);
			z = (z > 1.089f)
				? 1.089f
				: ((z < 0f) ? 0f : z);

			float fx = x * 1.05207785376f;
			float fy = y;
			float fz = z * 0.91827364554f;
			fx = (fx > 0.008856f)
				? FastMathf.Pow(fx, 0.3333333432674407958984375f)
				: (7.787f * fx + 0.13793103396892547607421875f);
			fy = (fy > 0.008856f)
				? FastMathf.Pow(fy, 0.3333333432674407958984375f)
				: (7.787f * fy + 0.13793103396892547607421875f);
			fz = (fz > 0.008856f)
				? FastMathf.Pow(fz, 0.3333333432674407958984375f)
				: (7.787f * fz + 0.13793103396892547607421875f);

			return new LABColor(
				l: (116.0f * fy) - 16f,
				a: 500.0f * (fx - fy),
				b: 200.0f * (fy - fz));
		}

		// converts LABColor to Color
		public static Color ToColor(LABColor lab)
		{
			float fy = (lab.l + 16f) * 0.00862068965f;
			float fx = fy + (lab.a * 0.002f);
			float fz = fy - (lab.b * 0.005f);

			float x = (fx > 0.20689655172f) 
				? 0.9505f * (fx * fx * fx) 
				: (fx - 0.13793103396892547607421875f) * 0.12206183114f;
			float y = (fy > 0.20689655172f) 
				? (fy * fy * fy) 
				: (fy - 0.13793103396892547607421875f) * 0.12841854933f;
			float z = (fz > 0.20689655172f) 
				? 1.0890f * (fz * fz * fz) 
				: (fz - 0.13793103396892547607421875f) * 0.13984780022f;

			float r = x * 3.2410f - y * 1.5374f - z * 0.4986f;
			float g = -x * 0.9692f + y * 1.8760f - z * 0.0416f;
			float b = x * 0.0556f - y * 0.2040f + z * 1.0570f;
			r = (r <= 0.0031308f) 
				? 12.92f * r 
				: 1.055f * Mathf.Pow(r, 0.4166666567325592041015625f) - 0.055f;
			g = (g <= 0.0031308f) 
				? 12.92f * g 
				: 1.055f * Mathf.Pow(g, 0.4166666567325592041015625f) - 0.055f;
			b = (b <= 0.0031308f) 
				? 12.92f * b 
				: 1.055f * Mathf.Pow(b, 0.4166666567325592041015625f) - 0.055f;

			return new Color(
				r: (r < 0) ? 0 : r,
				g: (g < 0) ? 0 : g,
				b: (b < 0) ? 0 : b);
		}

		// Converts LABColor to Color
		// Currently unused as accuracy is favoured over speed in this experiment.
		public static Color ToColor_Fast(LABColor lab)
		{
			float fy = (lab.l + 16f) * 0.00862068965f;
			float fx = fy + (lab.a * 0.002f);
			float fz = fy - (lab.b * 0.005f);

			float x = (fx > 0.20689655172f)
				? 0.9505f * (fx * fx * fx)
				: (fx - 0.13793103396892547607421875f) * 0.12206183114f;
			float y = (fy > 0.20689655172f)
				? (fy * fy * fy)
				: (fy - 0.13793103396892547607421875f) * 0.12841854933f;
			float z = (fz > 0.20689655172f)
				? 1.0890f * (fz * fz * fz)
				: (fz - 0.13793103396892547607421875f) * 0.13984780022f;

			float r = x * 3.2410f - y * 1.5374f - z * 0.4986f;
			float g = -x * 0.9692f + y * 1.8760f - z * 0.0416f;
			float b = x * 0.0556f - y * 0.2040f + z * 1.0570f;
			r = (r <= 0.0031308f)
				? 12.92f * r
				: 1.055f * FastMathf.Pow(r, 0.4166666567325592041015625f) - 0.055f;
			g = (g <= 0.0031308f)
				? 12.92f * g
				: 1.055f * FastMathf.Pow(g, 0.4166666567325592041015625f) - 0.055f;
			b = (b <= 0.0031308f)
				? 12.92f * b
				: 1.055f * FastMathf.Pow(b, 0.4166666567325592041015625f) - 0.055f;

			return new Color(
				r: (r < 0) ? 0 : r,
				g: (g < 0) ? 0 : g,
				b: (b < 0) ? 0 : b);
		}

		public Color ToColor() => LABColor.ToColor(this);

		public override string ToString() => $"l: {this.l}, a: {this.a}, b: {this.b}";

		public override int GetHashCode() => (l.GetHashCode() ^ a.GetHashCode() ^ b.GetHashCode());

		public bool Equals(LABColor other) => (this.l == other.l && this.a == other.a && this.b == other.b);

		public override bool Equals(object obj) => (obj != null && GetType() == obj.GetType() && (this == (LABColor)obj));

		public static bool operator ==(LABColor item1, LABColor item2) => (item1.l == item2.l && item1.a == item2.a && item1.b == item2.b);

		public static bool operator !=(LABColor item1, LABColor item2) => (item1.l != item2.l || item1.a != item2.a || item1.b != item2.b);
	}
}