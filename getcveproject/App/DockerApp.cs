using System.Text.Json;
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

        var outputDir = Path.Combine(Environment.CurrentDirectory, "output");
        Directory.CreateDirectory(outputDir);

        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(
            Path.Combine(outputDir, "images.json"),
            JsonSerializer.Serialize(images, jsonOptions));
        File.WriteAllText(
            Path.Combine(outputDir, "containers.json"),
            JsonSerializer.Serialize(containers, jsonOptions));
        File.WriteAllText(
            Path.Combine(outputDir, "version.json"),
            JsonSerializer.Serialize(version, jsonOptions));

        Console.WriteLine($"Loaded {images.Count} image(s).");
        foreach (var image in images)
            Console.WriteLine(image);

        return 0;
    }
}
