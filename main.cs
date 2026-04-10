using System;
using System.Diagnostics;
using System.Text.Json;
using SecurityProject.Commands;
using SecurityProject.Models;

class Program
{
    static int Main()
    {
        var dockerImageListCommand = ProcessStartInfoFactory.Create(Commands.DockerImageList);
        using var process = Process.Start(dockerImageListCommand);
        if (process == null)
        {
            Console.Error.WriteLine("Failed to start docker process.");
            return 1;
        }
        
        var stdout = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        var jsonLines = stdout.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

        var images = new List<Image>();
        foreach (var line in jsonLines)
        {
            var image = JsonSerializer.Deserialize<Image>(line);
            if (image != null)
                images.Add(image);
        }

        foreach (var image in images)
            Console.WriteLine(image);

        return 0;
    }
}
