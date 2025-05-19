using System.Text.Json.Serialization;

namespace atx2img.Data;

public class MeshData
{
    [JsonPropertyName("texNo")]
    public int TexNo { get; init; }

    [JsonPropertyName("offsetX")]
    public double OffsetX { get; init; }

    [JsonPropertyName("offsetY")]
    public double OffsetY { get; init; }

    [JsonPropertyName("srcOffsetX")]
    public double SrcOffsetX { get; init; }

    [JsonPropertyName("srcOffsetY")]
    public double SrcOffsetY { get; init; }

    [JsonPropertyName("texU1")]
    public double TexU1 { get; init; }

    [JsonPropertyName("texV1")]
    public double TexV1 { get; init; }

    [JsonPropertyName("texU2")]
    public double TexU2 { get; init; }

    [JsonPropertyName("texV2")]
    public double TexV2 { get; init; }

    [JsonPropertyName("viewX")]
    public double ViewX { get; init; }

    [JsonPropertyName("viewY")]
    public double ViewY { get; init; }

    [JsonPropertyName("width")]
    public double Width { get; init; }

    [JsonPropertyName("height")]
    public double Height { get; init; }
}