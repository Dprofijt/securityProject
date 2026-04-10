using System.Text.Json;
using System.Text.Json.Serialization;

namespace SecurityProject.Models;

public sealed class Image
{
    [JsonPropertyName("Containers")]
    public string? Containers { get; init; }

    [JsonPropertyName("CreatedAt")]
    public string? CreatedAt { get; init; }

    [JsonPropertyName("CreatedSince")]
    public string? CreatedSince { get; init; }

    [JsonPropertyName("Digest")]
    public string? Digest { get; init; }

    [JsonPropertyName("ID")]
    public string? Id { get; init; }

    [JsonPropertyName("Repository")]
    public string? Repository { get; init; }

    [JsonPropertyName("SharedSize")]
    public string? SharedSize { get; init; }

    [JsonPropertyName("Size")]
    public string? Size { get; init; }

    [JsonPropertyName("Tag")]
    public string? Tag { get; init; }

    [JsonPropertyName("UniqueSize")]
    public string? UniqueSize { get; init; }

    [JsonPropertyName("ScoutReport")]
    public string? ScoutReport { get; set; }

    [JsonPropertyName("Sbom")]
    public JsonDocument? Sbom { get; set; }

    public override string ToString()
    {
        var repo = string.IsNullOrWhiteSpace(Repository) ? "<none>" : Repository;
        var tag = string.IsNullOrWhiteSpace(Tag) ? "<none>" : Tag;
        var id = string.IsNullOrWhiteSpace(Id) ? "<none>" : Id;
        var size = string.IsNullOrWhiteSpace(Size) ? "<unknown>" : Size;
        var created = string.IsNullOrWhiteSpace(CreatedSince) ? "<unknown>" : CreatedSince;
        return $"{repo}:{tag} ({id}) size={size} created={created}";
    }
}
