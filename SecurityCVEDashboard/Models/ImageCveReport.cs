namespace SecurityCVEDashboard.Models;

public sealed class ImageCveReport
{
    public string ImageId { get; init; } = string.Empty;
    public string Repository { get; init; } = string.Empty;
    public string Tag { get; init; } = string.Empty;
    public IReadOnlyList<CveRecord> Vulnerabilities { get; init; } = [];
    public int TotalVulnerabilities { get; init; }
    public int CriticalCount { get; init; }
    public int HighCount { get; init; }
    public int MediumCount { get; init; }
    public int LowCount { get; init; }
    public double RiskScore { get; init; }
    public IReadOnlyList<string> TopVulnerablePackages { get; init; } = [];
}

public sealed class LinkedVulnerabilityGroup
{
    public string Key { get; init; } = string.Empty;
    public string PackageName { get; init; } = string.Empty;
    public string AffectedRange { get; init; } = string.Empty;
    public string FixedVersion { get; init; } = string.Empty;
    public IReadOnlyList<string> CveIds { get; init; } = [];
    public int Count { get; init; }
    public string EscalatedSeverity { get; init; } = "UNSPECIFIED";
    public bool IsCriticalChain { get; init; }
    public string ChainAssessment { get; init; } = string.Empty;
}

public sealed class VulnerabilityAnalysisSnapshot
{
    public DateTimeOffset GeneratedAt { get; init; }
    public IReadOnlyList<ImageCveReport> ImageReports { get; init; } = [];
    public IReadOnlyList<LinkedVulnerabilityGroup> LinkedGroups { get; init; } = [];
    public int TotalVulnerabilities { get; init; }
    public int CriticalCount { get; init; }
    public int HighCount { get; init; }
    public int MediumCount { get; init; }
    public int LowCount { get; init; }
    public IReadOnlyList<string> TopVulnerablePackages { get; init; } = [];
}
