namespace ReanimHelper;

public class Reanim
{
	public float Fps { get; set; }
	public Track[]? Tracks { get; set; }
}

public class Track
{
	public string? Name { get; set; }
	public Transform[]? TransForms { get; set; }
}

public class Transform
{
	/// <summary>
	/// frame type, -1: empty, 0: keyframe
	/// </summary>
	public float? Frame { get; set; }
	public string? Image { get; set; }
	public float? Alpha { get; set; }
	public float? X { get; set; }
	public float? Y { get; set; }
	public float? XScale { get; set; }
	public float? YScale { get; set; }

	/// <summary>
	/// degree
	/// </summary>
	public float? XRotation { get; set; }

	/// <summary>
	/// degree
	/// </summary>
	public float? YRotation { get; set; }
	public string? Font { get; set; }
	public string? Text { get; set; }

	public bool IsNullTransform()
	{
		return Frame == null &&
			   Image == null &&
			   Alpha == null &&
			   X == null &&
			   Y == null &&
			   XScale == null &&
			   YScale == null &&
			   XRotation == null &&
			   YRotation == null &&
			   Font == null &&
			   Text == null;
	}

	public bool IsEmptyTransform()
	{
		return Image == null &&
			   Alpha == null &&
			   X == null &&
			   Y == null &&
			   XScale == null &&
			   YScale == null &&
			   XRotation == null &&
			   YRotation == null &&
			   Font == null &&
			   Text == null;
	}
}