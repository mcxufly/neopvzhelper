using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Numerics;

namespace ReanimHelper;

public static class Transformation
{
	/// <summary>
	/// Tilt transformation matrix: {{cos(tx),-sin(ty)},{sin(tx),cos(ty)}}
	/// </summary>
	public class PositionTiltScaleAlpha
	{
		public float X { get; set; }
		public float Y { get; set; }

		[Range(0, float.MaxValue)]
		public float XScale { get; set; }
		[Range(0, float.MaxValue)]
		public float YScale { get; set; }

		/// <summary>
		/// radian
		/// </summary>
		[Range(-float.Pi, float.Pi)]
		public float XTilt { get; set; }

		/// <summary>
		/// radian
		/// </summary>
		[Range(-float.Pi, float.Pi)]
		public float YTilt { get; set; }

		[Range(0, 1)]
		public float Alpha { get; set; }

		public override string ToString()
		{
			return $"Position: ({X}, {Y}), Scale: ({XScale}, {YScale}), Tilt: ({XTilt}, {YTilt}), Alpha: {Alpha}";
		}

		public static PositionTiltScaleAlpha FromReaminTransForm(TransForm tf)
		{
			return new PositionTiltScaleAlpha()
			{
				X = tf.X ?? 0,
				Y = tf.Y ?? 0,
				XScale = tf.XScale ?? 1,
				YScale = tf.YScale ?? 1,
				XTilt = float.DegreesToRadians(tf.XTilt ?? 0),
				YTilt = float.DegreesToRadians(tf.YTilt ?? 0),
				Alpha = tf.Alpha ?? 1
			};
		}

		public Matrix3x2 ToTransform2D()
		{
			return new Matrix3x2
			{
				M11 = XScale * MathF.Cos(XTilt),
				M12 = XScale * MathF.Sin(XTilt),
				M21 = YScale * -MathF.Sin(YTilt),
				M22 = YScale * MathF.Cos(YTilt),
				M31 = X,
				M32 = Y
			};
		}

		public PositionRotationSkewScaleAlpha ToPRSSA()
		{
			float rotation = XTilt;
			float skew = XTilt - YTilt;

			return new PositionRotationSkewScaleAlpha()
			{
				X = X,
				Y = Y,
				XScale = XScale,
				YScale = YScale,
				Rotation = rotation,
				Skew = skew,
				Alpha = Alpha
			};
		}
	}

	/// <summary>
	/// Skew transformation matrix: {{1,-sin(-s)},{0,cos(-s)}}
	/// Rotation transformation matrix: {{cos(r),-sin(r)},{sin(r),cos(r)}}
	/// </summary>
	public class PositionRotationSkewScaleAlpha
	{
		public float X { get; set; }
		public float Y { get; set; }

		[Range(0, float.MaxValue)]
		public float XScale { get; set; }
		[Range(0, float.MaxValue)]
		public float YScale { get; set; }

		/// <summary>
		/// radian
		/// </summary>
		[Range(-float.Pi, float.Pi)]
		public float Rotation { get; set; }

		/// <summary>
		/// radian
		/// </summary>
		[Range(-float.Pi / 2, float.Pi / 2)]
		public float Skew { get; set; }
		[Range(0, 1)]
		public float Alpha { get; set; }

		public override string ToString()
		{
			return $"Position: ({X}, {Y}), Scale: ({XScale}, {YScale}), Rotation: {Rotation}, Skew: {Skew}, Alpha: {Alpha}";
		}

		public Matrix3x2 ToTransform2D()
		{
			return new Matrix3x2
			{
				M11 = XScale * MathF.Cos(Rotation),
				M12 = XScale * MathF.Sin(Rotation),
				M21 = YScale * MathF.Sin(Skew - Rotation),
				M22 = YScale * MathF.Cos(Rotation - Skew),
				M31 = X,
				M32 = Y
			};
		}
	}
}
