namespace SecurityCVEDashboard.Models;

public sealed class DockerImageInfo
{
    public string Containers { get; init; } = string.Empty;
    public string CreatedAt { get; init; } = string.Empty;
    public string CreatedSince { get; init; } = string.Empty;
    public string Digest { get; init; } = string.Empty;
    public string ID { get; init; } = string.Empty;
    public string Repository { get; init; } = string.Empty;
    public string SharedSize { get; init; } = string.Empty;
    public string Size { get; init; } = string.Empty;
    public string Tag { get; init; } = string.Empty;
    public string UniqueSize { get; init; } = string.Empty;
}
