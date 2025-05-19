using System.Text.Json.Serialization;

namespace atx2img.Data;

public class AtlasData
{
    [JsonPropertyName("Canvas")]
    public CanvasData? Canvas { get; init; }

    [JsonPropertyName("Block")]
    public List<BlockData>? Blocks { get; init; }
}