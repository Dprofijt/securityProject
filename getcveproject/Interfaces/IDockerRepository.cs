using SecurityProject.Models;

namespace SecurityProject.Repositories;

public interface IDockerRepository
{
    List<Image> GetImages();
    List<Container> GetContainers();
    string GetScoutCvesNginxLatest();
    DockerVersion? GetVersion();
}
