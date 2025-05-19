using System.Text.Json.Serialization;

namespace atx2img.Data;

public class AttributeData
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("x")]
    public double X { get; init; }

    [JsonPropertyName("y")]
    public double Y { get; init; }

    [JsonPropertyName("width")]
    public double Width { get; init; }

    [JsonPropertyName("height")]
    public double Height { get; init; }

    [JsonPropertyName("color")]
    public uint Color { get; init; } // Assuming color is an unsigned integer
}