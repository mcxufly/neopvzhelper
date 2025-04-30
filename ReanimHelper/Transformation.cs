using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace ReanimHelper;

public static class Transformation
{
	/// <summary>
	/// Rotation transformation matrix:s
	/// cos(kx) -sin(ky)
	/// sin(kx)  cos(ky)
	/// </summary>
	public class ReanimTransform
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float XScale { get; set; }
		public float YScale { get; set; }

		/// <summary>
		/// radian
		/// </summary>
		[Range(-float.Pi, float.Pi)]
		public float XRotation { get; set; }

		/// <summary>
		/// radian
		/// </summary>
		[Range(-float.Pi, float.Pi)]
		public float YRotation { get; set; }

		[Range(0, 1)]
		public float Alpha { get; set; }

		public override string ToString()
		{
			return $"Position: ({X}, {Y}), Scale: ({XScale}, {YScale}), Rotation: ({XRotation}, {YRotation}), Alpha: {Alpha}";
		}

		public static ReanimTransform FromReaminTransform(TransForm tf)
		{
			return new ReanimTransform()
			{
				X = tf.X ?? 0,
				Y = tf.Y ?? 0,
				XScale = tf.XScale ?? 1,
				YScale = tf.YScale ?? 1,
				XRotation = float.DegreesToRadians(tf.XRotation ?? 0),
				YRotation = float.DegreesToRadians(tf.YRotation ?? 0),
				Alpha = tf.Alpha ?? 1
			};
		}

		public Matrix3x2 ToTransform2D()
		{
			return new Matrix3x2
			{
				M11 = XScale * MathF.Cos(XRotation),
				M12 = XScale * MathF.Sin(XRotation),
				M21 = YScale * -MathF.Sin(YRotation),
				M22 = YScale * MathF.Cos(YRotation),
				M31 = X,
				M32 = Y
			};
		}

		public Godot2DTransform ToGodot2DTransform()
		{
			return new Godot2DTransform()
			{
				X = X,
				Y = Y,
				XScale = XScale,
				YScale = YScale,
				Rotation = XRotation,
				Skew = XRotation - YRotation,
				Alpha = Alpha
			};
		}
	}

	/// <summary>
	/// Rotation transformation matrix:
	/// cos(r) -sin(r)
	/// sin(r)  cos(r)
	/// Skew(Y axi rotation) transformation matrix:
	/// 1 -sin(-s)
	/// 0  cos(-s)
	/// </summary>
	public class Godot2DTransform
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float XScale { get; set; }
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
