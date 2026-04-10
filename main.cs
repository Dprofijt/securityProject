using System;
using SecurityProject.Models;
using SecurityProject.Repositories;
using SecurityProject.Commands;

class Program
{
    static int Main()
    {
        IDockerRepository repo = new DockerRepository();
        var images = repo.GetAll<Image>(Commands.DockerImageList);

        Console.WriteLine($"Loaded {images.Count} image(s).");
        foreach (var image in images)
            Console.WriteLine(image);

        return 0;
    }
}
