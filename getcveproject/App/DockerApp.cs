using System.Text.Json;
using SecurityProject.Models;
using SecurityProject.Repositories;

namespace SecurityProject.App;

public sealed class DockerApp
{
    private readonly IDockerRepository _repo;

    public DockerApp()
    {
        _repo = new DockerRepository();
    }

    public int Run()
    {
        var images = _repo.GetImages();
        var containers = _repo.GetContainers();
        var version = _repo.GetVersion();

        foreach (var image in images)
        {
            var imageRef = BuildImageRef(image);
            if (!string.IsNullOrWhiteSpace(imageRef))
            {
                image.ScoutReport = _repo.GetScoutCves(imageRef);
                image.Sbom = _repo.GetSbom(imageRef);
            }
        }

        var docker = new Docker
        {
            Images = images,
            Containers = containers,
            Version = version?.Server?.Version ?? version?.Client?.Version
        };

        var outputDir = Path.Combine(Environment.CurrentDirectory, "output");
        Directory.CreateDirectory(outputDir);

        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(
            Path.Combine(outputDir, "docker.json"),
            JsonSerializer.Serialize(docker, jsonOptions));

        Console.WriteLine($"Loaded {images.Count} image(s).");
        foreach (var image in images)
            Console.WriteLine(image);

        return 0;
    }

    private static string? BuildImageRef(Image image)
    {
        if (!string.IsNullOrWhiteSpace(image.Repository))
        {
            var tag = string.IsNullOrWhiteSpace(image.Tag) ? "latest" : image.Tag;
            if (image.Repository != "<none>")
                return $"{image.Repository}:{tag}";
        }

        return string.IsNullOrWhiteSpace(image.Id) ? null : image.Id;
    }
}
