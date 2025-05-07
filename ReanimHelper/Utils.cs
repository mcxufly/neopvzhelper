using System.Formats.Tar;
using System.IO.Compression;
using System.Text;
using static ReanimHelper.Transformation;

namespace ReanimHelper;

public static class Utils
{
	public static Reanim LoadFromCompiled(string compliedFile)
	{
		using BinaryReader sourceData = new BinaryReader(File.Open(compliedFile, FileMode.Open));
		using BinaryReader data = new BinaryReader(new MemoryStream());

		if (sourceData.ReadInt32() == -559022380)
		{
			int size = sourceData.ReadInt32();
			using ZLibStream zLibStream = new ZLibStream(sourceData.BaseStream, CompressionMode.Decompress);
			zLibStream.CopyTo(data.BaseStream);
		}
		else
		{
			sourceData.BaseStream.CopyTo(data.BaseStream);
		}


		data.BaseStream.Position = 0;
		data.Skip(8);
		int tracksNumber = data.ReadInt32();
		Reanim reanim = new Reanim(tracksNumber);
		// reanim.Tracks = new Track[tracksNumber];
		reanim.Fps = data.ReadSingle();
		data.Skip(4);
		data.CheckInt32(0xC);

		for (int i = 0; i < tracksNumber; i++)
		{
			data.Skip(8);
			Track track = new Track(data.ReadInt32());
			// track.Transforms = new Transform[data.ReadInt32()];
			reanim.Tracks[i] = track;
		}

		for (int i = 0; i < tracksNumber; i++)
		{
			Track track = reanim.Tracks[i];
			track.Name = data.ReadStringByInt32Head() ?? "";
			data.CheckInt32(0x2C);
			int length = track.Transforms.Length;
			for (int j = 0; j < length; j++)
			{
				Transform tf = new Transform();
				float f;

				f = data.ReadSingle();
				tf.X = f == -10000f ? null : f;
				f = data.ReadSingle();
				tf.Y = f == -10000f ? null : f;
				f = data.ReadSingle();
				tf.XRotation = f == -10000f ? null : f;
				f = data.ReadSingle();
				tf.YRotation = f == -10000f ? null : f;
				f = data.ReadSingle();
				tf.XScale = f == -10000f ? null : f;
				f = data.ReadSingle();
				tf.YScale = f == -10000f ? null : f;
				f = data.ReadSingle();
				tf.Frame = f == -10000f ? null : f;
				f = data.ReadSingle();
				tf.Alpha = f == -10000f ? null : f;
				data.Skip(12);
				track.Transforms[j] = tf;
			}

			for (int j = 0; j < length; j++)
			{
				Transform tf = track.Transforms[j];

				tf.Image = data.ReadStringByInt32Head();
				tf.Font = data.ReadStringByInt32Head();
				tf.Text = data.ReadStringByInt32Head();
			}
		}

		return reanim;
	}

	public static void CheckInt32(this BinaryReader reader, int t)
	{
		if (!reader.ReadInt32().Equals(t)) throw new Exception("Miss Metch");
	}

	public static void Skip(this BinaryReader reader, int btyes)
	{
		reader.BaseStream.Position += btyes;
	}

	public static string? ReadStringByInt32Head(this BinaryReader reader)
	{
		byte[] chars = reader.ReadBytes(reader.ReadInt32());
		if (chars.Length == 0)
			return null;
		return Encoding.UTF8.GetString(chars);
	}

	public static List<Transform> PrepareTransforms(Transform[] transforms)
	{
		bool end = false;
		for (int i = 0; i < transforms.Length; i++)
		{
			Transform transform = transforms[i];
			if (transform.Frame == -1)
			{
				end = true;
			}
			else if (transform.Frame == 0)
			{
				end = false;
			}

			Transform prevTf = i == 0 ? transform : transforms[i - 1];
			float frame = end ? -i : i;
			transform.Frame = frame;
			transform.Alpha ??= prevTf.Alpha;
			transform.X ??= prevTf.X;
			transform.Y ??= prevTf.Y;
			transform.XScale ??= prevTf.XScale;
			transform.YScale ??= prevTf.YScale;
			transform.XRotation ??= prevTf.XRotation;
			transform.YRotation ??= prevTf.YRotation;
		}

		List<Transform> newTransforms = new List<Transform>();

		for (int i = 0; i < transforms.Length; i++)
		{
			Transform transform = transforms[i];
			if (transform.Frame >= 0)
			{
				newTransforms.Add(transform);
			}
		}

		return newTransforms;
	}

	public static void ConvertAnimation(Reanim reanim, StreamWriter outStream)
	{
		outStream.WriteLine($"FPS: {reanim.Fps}");
		outStream.WriteLine($"Length: {reanim.Tracks[0].Transforms.Length}");
		outStream.WriteLine();

		foreach (Track track in reanim.Tracks)
		{
			List<Transform> list = PrepareTransforms(track.Transforms);
			List<string> sprites = new List<string>();
			List<float> frames = new List<float>();
			List<float> times = new List<float>();
			List<int> transitions = new List<int>();
			List<string> values = new List<string>();
			List<string> fonts = new List<string>();
			List<string> texts = new List<string>();
			List<string> colors = new List<string>();

			foreach (Transform tf in list)
			{
				if (tf.Frame == -1)
				{
					continue;
				}

				if (tf.Image != null)
				{
					sprites.Add(tf.Image + $" {tf.Frame}");
				}
				if (tf.Font != null)
				{
					fonts.Add(tf.Font + $" {tf.Frame}");
				}
				if (tf.Text != null)
				{
					texts.Add(tf.Text + $" {tf.Frame}");
				}

				ReanimTransform rt = ReanimTransform.FromReaminTransform(tf);
				frames.Add(tf.Frame ?? 0);
				times.Add((tf.Frame ?? 0) * 1 / reanim.Fps);
				transitions.Add(1);
				values.Add(GodotTransform2DString(rt.ToTransform2D()));
				if (tf.Alpha != null)
				{
					colors.Add($"Color(1, 1, 1, {tf.Alpha ?? 0})");
				}
			}

			outStream.WriteLine($"Track: {track.Name}");
			outStream.WriteLine($"sprites: {string.Join(", ", sprites)}");
			outStream.WriteLine($"frames: {string.Join(", ", frames)}");

			outStream.WriteLine("{");
			outStream.WriteLine($"\"times\": PackedFloat32Array({string.Join(", ", times)}),");
			outStream.WriteLine($"\"transitions\": PackedFloat32Array({string.Join(", ", transitions)}),");
			outStream.WriteLine("\"update\": 0,");
			outStream.WriteLine($"\"values\": [{string.Join(", ", values)}]");
			outStream.WriteLine("}");

			if (colors.Count != 0)
			{
				outStream.WriteLine("{");
				outStream.WriteLine($"\"times\": PackedFloat32Array({string.Join(", ", times)}),");
				outStream.WriteLine($"\"transitions\": PackedFloat32Array({string.Join(", ", transitions)}),");
				outStream.WriteLine("\"update\": 0,");
				outStream.WriteLine($"\"values\": [{string.Join(", ", colors)}]");
				outStream.WriteLine("}");
			}

			if (fonts.Count != 0)
			{
				outStream.WriteLine($"fonts: {string.Join(", ", fonts)}");
			}
			if (texts.Count != 0)
			{
				outStream.WriteLine($"texts: {string.Join(", ", texts)}");
			}
			outStream.WriteLine();
		}
	}
}
