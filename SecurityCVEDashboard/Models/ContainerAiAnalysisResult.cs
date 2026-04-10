namespace SecurityCVEDashboard.Models;

public sealed class ContainerAiAnalysisResult
{
    public bool IsSuccess { get; init; }
    public string Analysis { get; init; } = string.Empty;
    public string Error { get; init; } = string.Empty;
}
