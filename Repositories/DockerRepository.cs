using System.Text.Json;
using SecurityProject.Commands;

namespace SecurityProject.Repositories;

public sealed class DockerRepository : IDockerRepository
{
    public List<T> GetAll<T>(CommandSpec command)
    {
        var startInfo = ProcessStartInfoFactory.Create(command);
        using var process = System.Diagnostics.Process.Start(startInfo);
        if (process == null)
            return [];

        var stdout = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        var results = new List<T>();
        var lines = stdout.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var item = JsonSerializer.Deserialize<T>(line);
            if (item != null)
                results.Add(item);
        }

        return results;
    }
}
