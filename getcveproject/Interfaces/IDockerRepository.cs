using System.Text.Json;
using SecurityProject.Models;

namespace SecurityProject.Repositories;

public interface IDockerRepository
{
    List<Image> GetImages();
    List<Container> GetContainers();
    string GetScoutCves(string imageRef);
    JsonDocument? GetSbom(string imageRef);
    DockerVersion? GetVersion();
}
