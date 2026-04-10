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

    public string GetScoutCves(string imageRef)
    {
        var (ExitCode, StdOut, StdErr) = Run(DockerCommands.DockerScoutCves(imageRef));
        return StdOut;
    }

    public JsonDocument? GetSbom(string imageRef)
    {
        return RunJsonDocument(DockerCommands.DockerScoutSbom(imageRef));
    }

    public DockerVersion? GetVersion()
    {
        return RunJson<DockerVersion>(DockerCommands.DockerVersion);
    }

    private static List<T> GetAll<T>(CommandSpec command)
    {
        var (ExitCode, StdOut, StdErr) = Run(command);

        var results = new List<T>();
        var lines = StdOut.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var item = JsonSerializer.Deserialize<T>(line);
            if (item != null)
                results.Add(item);
        }

        return results;
    }

    private static T? RunJson<T>(CommandSpec command)
    {
        var (ExitCode, StdOut, StdErr) = Run(command);
        if (string.IsNullOrWhiteSpace(StdOut))
            return default;

        return JsonSerializer.Deserialize<T>(StdOut);
    }

    private static JsonDocument? RunJsonDocument(CommandSpec command)
    {
        var (ExitCode, StdOut, StdErr) = Run(command);
        if (string.IsNullOrWhiteSpace(StdOut))
            return default;

        return JsonDocument.Parse(StdOut);
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
