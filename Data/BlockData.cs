using System.Text.Json.Serialization;

namespace atx2img.Data;

public class BlockData
{
    [JsonPropertyName("filename")]
    public string? Filename { get; init; }

    [JsonPropertyName("filenameOld")]
    public string? FilenameOld { get; init; }

    [JsonPropertyName("blend")]
    public string? Blend { get; init; }

    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("anchorX")]
    public double AnchorX { get; init; }

    [JsonPropertyName("anchorY")]
    public double AnchorY { get; init; }

    [JsonPropertyName("width")]
    public double Width { get; init; }

    [JsonPropertyName("height")]
    public double Height { get; init; }

    [JsonPropertyName("offsetX")]
    public double OffsetX { get; init; }

    [JsonPropertyName("offsetY")]
    public double OffsetY { get; init; }

    [JsonPropertyName("priority")]
    public int Priority { get; init; }

    [JsonPropertyName("Mesh")]
    public List<MeshData>? Mesh { get; init; }

    [JsonPropertyName("Attribute")]
    public List<AttributeData>? Attribute { get; init; }
}