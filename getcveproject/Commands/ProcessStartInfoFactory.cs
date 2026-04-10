using System.Diagnostics;

namespace SecurityProject.Commands;

public static class ProcessStartInfoFactory
{
    public static ProcessStartInfo Create(CommandSpec spec)
    {
        var psi = new ProcessStartInfo
        {
            FileName = spec.FileName,
            RedirectStandardOutput = spec.RedirectStdOut,
            RedirectStandardError = spec.RedirectStdErr,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (var arg in spec.Arguments)
            psi.ArgumentList.Add(arg);

        if (!string.IsNullOrWhiteSpace(spec.WorkingDirectory))
            psi.WorkingDirectory = spec.WorkingDirectory;

        if (spec.Environment != null)
        {
            foreach (var kvp in spec.Environment)
                psi.Environment[kvp.Key] = kvp.Value;
        }

        return psi;
    }
}
