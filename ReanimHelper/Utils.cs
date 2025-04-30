using System.IO.Compression;
using System.Text;

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

		Reanim reanim = new Reanim();
		data.BaseStream.Position = 0;
		data.Skip(8);
		int tracksNumber = data.ReadInt32();
		reanim.Tracks = new Track[tracksNumber];
		reanim.Fps = data.ReadSingle();
		data.Skip(4);
		data.CheckInt32(0xC);

		for (int i = 0; i < tracksNumber; i++)
		{
			data.Skip(8);
			Track track = new Track();
			track.TransForms = new TransForm[data.ReadInt32()];
			reanim.Tracks[i] = track;
		}

		for (int i = 0; i < tracksNumber; i++)
		{
			Track track = reanim.Tracks[i];
			track.Name = data.ReadStringByInt32Head();
			data.CheckInt32(0x2C);
			int length = track.TransForms.Length;
			for (int j = 0; j < length; j++)
			{
				TransForm tf = new TransForm();
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
				track.TransForms[j] = tf;
			}

			for (int j = 0; j < length; j++)
			{
				TransForm tf = track.TransForms[j];
				string str;

				str = data.ReadStringByInt32Head();
				tf.Image = string.IsNullOrEmpty(str) ? null : str;
				str = data.ReadStringByInt32Head();
				tf.Font = string.IsNullOrEmpty(str) ? null : str;
				str = data.ReadStringByInt32Head();
				tf.Text = string.IsNullOrEmpty(str) ? null : str;
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

	public static string ReadStringByInt32Head(this BinaryReader reader)
	{
		byte[] chars = reader.ReadBytes(reader.ReadInt32());
		return Encoding.UTF8.GetString(chars);
	}
}
