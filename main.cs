using SecurityProject.App;
using SecurityProject.Repositories;

class Program
{
    static int Main()
    {
        DockerApp dockerApp = new();
        return dockerApp.Run();
    }
}
