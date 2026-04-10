using System.Text;
using System.Text.Json;
using System.Net.Http.Json;
using SecurityCVEDashboard.Models;

namespace SecurityCVEDashboard.Services;

public sealed class GoogleCveAnalysisService
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IConfiguration configuration;
    private readonly ILogger<GoogleCveAnalysisService> logger;

    public GoogleCveAnalysisService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<GoogleCveAnalysisService> logger)
    {
        this.httpClientFactory = httpClientFactory;
        this.configuration = configuration;
        this.logger = logger;
    }

    public async Task<ContainerAiAnalysisResult> AnalyzeContainerAsync(
        DockerImageInfo image,
        ImageCveReport report,
        CancellationToken cancellationToken = default)
    {
        var apiKey = configuration["GoogleAgent:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return new ContainerAiAnalysisResult
            {
                IsSuccess = false,
                Error = "Google API key is missing. Configure GoogleAgent:ApiKey in appsettings or user secrets."
            };
        }

        var model = configuration["GoogleAgent:Model"];
        if (string.IsNullOrWhiteSpace(model))
        {
            model = "gemini-1.5-flash";
        }

        var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={Uri.EscapeDataString(apiKey)}";

        var payload = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = BuildPrompt(image, report)
                        }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.2,
                topP = 0.8
            }
        };

        try
        {
            using var client = httpClientFactory.CreateClient();
            using var response = await client.PostAsJsonAsync(endpoint, payload, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Google CVE analysis request failed with status {StatusCode}. Body: {Body}",
                    response.StatusCode,
                    responseBody);

                return new ContainerAiAnalysisResult
                {
                    IsSuccess = false,
                    Error = $"Google analysis failed ({(int)response.StatusCode})."
                };
            }

            var analysis = ExtractAnalysisText(responseBody);
            if (string.IsNullOrWhiteSpace(analysis))
            {
                return new ContainerAiAnalysisResult
                {
                    IsSuccess = false,
                    Error = "Google analysis did not return text output."
                };
            }

            return new ContainerAiAnalysisResult
            {
                IsSuccess = true,
                Analysis = analysis.Trim()
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Google CVE analysis failed for image {ImageId}", image.ID);
            return new ContainerAiAnalysisResult
            {
                IsSuccess = false,
                Error = "Google analysis request failed due to an unexpected error."
            };
        }
    }

    private static string BuildPrompt(DockerImageInfo image, ImageCveReport report)
    {
        var vulnerabilities = report.Vulnerabilities.Take(40).ToList();

        var sb = new StringBuilder();
        sb.AppendLine("You are a security analyst. Analyze CVEs for one container image.");
        sb.AppendLine("Return concise, practical remediation guidance.");
        sb.AppendLine();
        sb.AppendLine("Output format:");
        sb.AppendLine("1) Executive summary (max 5 bullet points)");
        sb.AppendLine("2) Potential exploit chains (if any)");
        sb.AppendLine("3) Priority remediation plan (ordered)");
        sb.AppendLine("4) Validation checks after remediation");
        sb.AppendLine();
        sb.AppendLine("Container:");
        sb.AppendLine($"- Repository: {image.Repository}");
        sb.AppendLine($"- Tag: {image.Tag}");
        sb.AppendLine($"- Image ID: {image.ID}");
        sb.AppendLine($"- Running containers: {image.Containers}");
        sb.AppendLine();
        sb.AppendLine("Risk metrics:");
        sb.AppendLine($"- Total CVEs: {report.TotalVulnerabilities}");
        sb.AppendLine($"- Critical: {report.CriticalCount}, High: {report.HighCount}, Medium: {report.MediumCount}, Low: {report.LowCount}");
        sb.AppendLine($"- Risk score: {report.RiskScore:0.0}");
        sb.AppendLine();
        sb.AppendLine("CVEs:");

        foreach (var cve in vulnerabilities)
        {
            sb.AppendLine($"- {cve.Id} | Severity: {cve.EffectiveSeverity} | Package: {cve.PackageName} | Installed: {cve.Version} | Affected: {cve.AffectedRange} | Fixed: {cve.FixedVersion} | Score: {cve.AdjustedScore:0.0}");
        }

        if (report.Vulnerabilities.Count > vulnerabilities.Count)
        {
            sb.AppendLine($"- ... {report.Vulnerabilities.Count - vulnerabilities.Count} more CVEs not listed for brevity");
        }

        return sb.ToString();
    }

    private static string ExtractAnalysisText(string responseBody)
    {
        using var document = JsonDocument.Parse(responseBody);

        if (!document.RootElement.TryGetProperty("candidates", out var candidates)
            || candidates.ValueKind != JsonValueKind.Array
            || candidates.GetArrayLength() == 0)
        {
            return string.Empty;
        }

        var firstCandidate = candidates[0];
        if (!firstCandidate.TryGetProperty("content", out var content)
            || !content.TryGetProperty("parts", out var parts)
            || parts.ValueKind != JsonValueKind.Array)
        {
            return string.Empty;
        }

        var texts = parts
            .EnumerateArray()
            .Where(part => part.TryGetProperty("text", out _))
            .Select(part => part.GetProperty("text").GetString())
            .Where(text => !string.IsNullOrWhiteSpace(text));

        return string.Join(Environment.NewLine, texts);
    }
}
