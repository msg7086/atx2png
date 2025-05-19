using System.Text.Json.Serialization;

namespace atx2img.Data;

public class CanvasData
{
    [JsonPropertyName("Width")]
    public int Width { get; init; }

    [JsonPropertyName("Height")]
    public int Height { get; init; }
}