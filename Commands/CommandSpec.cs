namespace SecurityProject.Commands;

public sealed record CommandSpec(
    string FileName,
    IReadOnlyList<string> Arguments,
    string? WorkingDirectory = null,
    IReadOnlyDictionary<string, string>? Environment = null,
    bool RedirectStdOut = true,
    bool RedirectStdErr = true
);
