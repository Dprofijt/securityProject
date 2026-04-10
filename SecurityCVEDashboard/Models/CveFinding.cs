namespace SecurityCVEDashboard.Models;

public sealed class CveFinding
{
    public string Id { get; init; } = string.Empty;
    public string Severity { get; init; } = "UNSPECIFIED";
    public string Package { get; init; } = string.Empty;
    public string AffectedRange { get; init; } = string.Empty;
    public string FixedVersion { get; init; } = string.Empty;
    public double SecuritySeverityScore { get; init; }
}
