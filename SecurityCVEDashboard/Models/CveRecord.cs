namespace SecurityCVEDashboard.Models;

public sealed class CveRecord
{
    public string Id { get; init; } = string.Empty;
    public string Severity { get; init; } = "UNSPECIFIED";
    public string EffectiveSeverity { get; init; } = "UNSPECIFIED";
    public string Package { get; init; } = string.Empty;
    public string PackageName { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public string AffectedRange { get; init; } = string.Empty;
    public string FixedVersion { get; init; } = string.Empty;
    public double CvssScore { get; init; }
    public double AdjustedScore { get; init; }
    public IReadOnlyList<string> References { get; init; } = [];
}
