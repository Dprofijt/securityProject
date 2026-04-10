namespace SecurityProject.Models;

public sealed class Docker
{
    public List<Image> Images { get; init; } = [];
    public List<Container> Containers { get; init; } = [];
    public string? Version { get; init; }
}
