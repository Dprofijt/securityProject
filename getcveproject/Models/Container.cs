using System.Text.Json.Serialization;

namespace SecurityProject.Models;

public sealed class Container
{
    [JsonPropertyName("ID")]
    public string? Id { get; init; }

    [JsonPropertyName("Image")]
    public string? Image { get; init; }

    [JsonPropertyName("Command")]
    public string? Command { get; init; }

    [JsonPropertyName("CreatedAt")]
    public string? CreatedAt { get; init; }

    [JsonPropertyName("RunningFor")]
    public string? RunningFor { get; init; }

    [JsonPropertyName("Status")]
    public string? Status { get; init; }

    [JsonPropertyName("Ports")]
    public string? Ports { get; init; }

    [JsonPropertyName("Names")]
    public string? Names { get; init; }

    [JsonPropertyName("Size")]
    public string? Size { get; init; }

    [JsonPropertyName("Labels")]
    public string? Labels { get; init; }

    [JsonPropertyName("Mounts")]
    public string? Mounts { get; init; }

    [JsonPropertyName("LocalVolumes")]
    public string? LocalVolumes { get; init; }

    [JsonPropertyName("Networks")]
    public string? Networks { get; init; }

    public override string ToString()
    {
        var id = string.IsNullOrWhiteSpace(Id) ? "<none>" : Id;
        var image = string.IsNullOrWhiteSpace(Image) ? "<none>" : Image;
        var name = string.IsNullOrWhiteSpace(Names) ? "<none>" : Names;
        var status = string.IsNullOrWhiteSpace(Status) ? "<unknown>" : Status;
        return $"{name} ({id}) image={image} status={status}";
    }
}
