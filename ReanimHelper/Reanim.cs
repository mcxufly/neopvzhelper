using System.Text.Json.Serialization;

namespace ReanimHelper;

public class Reanim
{
	public float Fps { get; set; }
	public Track[] Tracks { get; set; }
}

public class Track
{
	public string Name { get; set; }
	public TransForm[] TransForms { get; set; }
}

public class TransForm
{
	/// <summary>
	/// frame type, -1: empty, 0: keyframe
	/// </summary>
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public float? Frame { get; set; }
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? Image { get; set; }
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public float? Alpha { get; set; }
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public float? X { get; set; }
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public float? Y { get; set; }
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public float? XScale { get; set; }
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public float? YScale { get; set; }

	/// <summary>
	/// degree
	/// </summary>
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public float? XTilt { get; set; }

	/// <summary>
	/// degree
	/// </summary>
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public float? YTilt { get; set; }
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? Font { get; set; }
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? Text { get; set; }
}