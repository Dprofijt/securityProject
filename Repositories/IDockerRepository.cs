using SecurityProject.Commands;

namespace SecurityProject.Repositories;

public interface IDockerRepository
{
    List<T> GetAll<T>(CommandSpec command);
}
