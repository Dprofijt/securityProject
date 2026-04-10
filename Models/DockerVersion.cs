using System.Text.Json.Serialization;

namespace SecurityProject.Models;

public sealed class DockerVersion
{
    [JsonPropertyName("Client")]
    public DockerClientVersion? Client { get; init; }

    [JsonPropertyName("Server")]
    public DockerServerVersion? Server { get; init; }
}

public sealed class DockerClientVersion
{
    [JsonPropertyName("Version")]
    public string? Version { get; init; }

    [JsonPropertyName("ApiVersion")]
    public string? ApiVersion { get; init; }

    [JsonPropertyName("DefaultAPIVersion")]
    public string? DefaultApiVersion { get; init; }

    [JsonPropertyName("GitCommit")]
    public string? GitCommit { get; init; }

    [JsonPropertyName("GoVersion")]
    public string? GoVersion { get; init; }

    [JsonPropertyName("Os")]
    public string? Os { get; init; }

    [JsonPropertyName("Arch")]
    public string? Arch { get; init; }

    [JsonPropertyName("BuildTime")]
    public string? BuildTime { get; init; }

    [JsonPropertyName("Context")]
    public string? Context { get; init; }
}

public sealed class DockerServerVersion
{
    [JsonPropertyName("Platform")]
    public DockerPlatform? Platform { get; init; }

    [JsonPropertyName("Version")]
    public string? Version { get; init; }

    [JsonPropertyName("ApiVersion")]
    public string? ApiVersion { get; init; }

    [JsonPropertyName("MinAPIVersion")]
    public string? MinApiVersion { get; init; }

    [JsonPropertyName("Os")]
    public string? Os { get; init; }

    [JsonPropertyName("Arch")]
    public string? Arch { get; init; }

    [JsonPropertyName("Components")]
    public List<DockerComponent> Components { get; init; } = [];

    [JsonPropertyName("GitCommit")]
    public string? GitCommit { get; init; }

    [JsonPropertyName("GoVersion")]
    public string? GoVersion { get; init; }

    [JsonPropertyName("KernelVersion")]
    public string? KernelVersion { get; init; }

    [JsonPropertyName("BuildTime")]
    public string? BuildTime { get; init; }
}

public sealed class DockerPlatform
{
    [JsonPropertyName("Name")]
    public string? Name { get; init; }
}

public sealed class DockerComponent
{
    [JsonPropertyName("Name")]
    public string? Name { get; init; }

    [JsonPropertyName("Version")]
    public string? Version { get; init; }

    [JsonPropertyName("Details")]
    public Dictionary<string, string>? Details { get; init; }
}
