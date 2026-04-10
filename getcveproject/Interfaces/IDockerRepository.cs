using SecurityProject.Models;

namespace SecurityProject.Repositories;

public interface IDockerRepository
{
    List<Image> GetImages();
    List<Container> GetContainers();
    string GetScoutCvesNginxLatest();
    string GetScoutCves(string imageRef);
    DockerVersion? GetVersion();
}
