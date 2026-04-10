using System.Text.Json;
using SecurityProject.Commands;
using SecurityProject.Models;

namespace SecurityProject.Repositories;

public sealed class DockerRepository : IDockerRepository
{
    public List<Image> GetImages()
    {
        return GetAll<Image>(DockerCommands.DockerImageList);
    }

    public List<Container> GetContainers()
    {
        return GetAll<Container>(DockerCommands.DockerContainerList);
    }

    public string GetScoutCvesNginxLatest()
    {
        var result = Run(DockerCommands.DockerScoutCvesNginxLatest);
        return result.StdOut;
    }

    private static List<T> GetAll<T>(CommandSpec command)
    {
        var result = Run(command);

        var results = new List<T>();
        var lines = result.StdOut.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var item = JsonSerializer.Deserialize<T>(line);
            if (item != null)
                results.Add(item);
        }

        return results;
    }

    private static (int ExitCode, string StdOut, string StdErr) Run(CommandSpec command)
    {
        var startInfo = ProcessStartInfoFactory.Create(command);
        using var process = System.Diagnostics.Process.Start(startInfo);
        if (process == null)
            return (-1, string.Empty, "Failed to start docker process.");

        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();

        return (process.ExitCode, stdout, stderr);
    }
}
