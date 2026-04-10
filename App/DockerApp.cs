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

        Console.WriteLine($"Loaded {images.Count} image(s).");
        foreach (var image in images)
            Console.WriteLine(image);

        return 0;
    }
}
