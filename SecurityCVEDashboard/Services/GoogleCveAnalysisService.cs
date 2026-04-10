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

        var configuredApiVersion = configuration["GoogleAgent:ApiVersion"];
        var apiVersion = string.IsNullOrWhiteSpace(configuredApiVersion) ? "v1beta" : configuredApiVersion;

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

            var primaryResponse = await SendGenerateContentAsync(client, apiVersion, model, apiKey, payload, cancellationToken);
            if (!primaryResponse.Success && primaryResponse.StatusCode == System.Net.HttpStatusCode.Forbidden && apiVersion.Equals("v1beta", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogInformation("Retrying Google CVE analysis with API version v1 after 403 from v1beta.");
                var retryResponse = await SendGenerateContentAsync(client, "v1", model, apiKey, payload, cancellationToken);
                if (retryResponse.Success)
                {
                    return new ContainerAiAnalysisResult
                    {
                        IsSuccess = true,
                        Analysis = retryResponse.Analysis.Trim()
                    };
                }

                return BuildFailureResult(retryResponse.StatusCode, retryResponse.ErrorMessage);
            }

            if (!primaryResponse.Success)
            {
                return BuildFailureResult(primaryResponse.StatusCode, primaryResponse.ErrorMessage);
            }

            return new ContainerAiAnalysisResult
            {
                IsSuccess = true,
                Analysis = primaryResponse.Analysis.Trim()
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

    private ContainerAiAnalysisResult BuildFailureResult(System.Net.HttpStatusCode statusCode, string errorMessage)
    {
        var status = (int)statusCode;
        var userSafeError = string.IsNullOrWhiteSpace(errorMessage)
            ? $"Google analysis failed ({status})."
            : $"Google analysis failed ({status}): {errorMessage}";

        return new ContainerAiAnalysisResult
        {
            IsSuccess = false,
            Error = userSafeError
        };
    }

    private async Task<GoogleGenerateContentResponse> SendGenerateContentAsync(
        HttpClient client,
        string apiVersion,
        string model,
        string apiKey,
        object payload,
        CancellationToken cancellationToken)
    {
        var endpoint = $"https://generativelanguage.googleapis.com/{apiVersion}/models/{model}:generateContent";

        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = JsonContent.Create(payload)
        };
        request.Headers.Add("x-goog-api-key", apiKey);

        using var response = await client.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var googleErrorMessage = TryExtractGoogleErrorMessage(responseBody);
            logger.LogWarning(
                "Google CVE analysis request failed with status {StatusCode}. Body: {Body}",
                response.StatusCode,
                responseBody);

            return GoogleGenerateContentResponse.Failure(response.StatusCode, googleErrorMessage);
        }

        var analysis = ExtractAnalysisText(responseBody);
        if (string.IsNullOrWhiteSpace(analysis))
        {
            return GoogleGenerateContentResponse.Failure(response.StatusCode, "Google analysis did not return text output.");
        }

        return GoogleGenerateContentResponse.Success(analysis);
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

        var textParts = parts
            .EnumerateArray()
            .Select(part => TryExtractPartText(part))
            .Where(text => !string.IsNullOrWhiteSpace(text));

        return string.Join(Environment.NewLine, textParts);
    }

    private static string? TryExtractPartText(JsonElement part)
    {
        if (part.TryGetProperty("text", out var textElement) && textElement.ValueKind == JsonValueKind.String)
        {
            return textElement.GetString();
        }

        if (part.TryGetProperty("outputText", out var outputTextElement) && outputTextElement.ValueKind == JsonValueKind.String)
        {
            return outputTextElement.GetString();
        }

        if (part.TryGetProperty("inlineData", out var inlineData)
            && inlineData.ValueKind == JsonValueKind.Object
            && inlineData.TryGetProperty("data", out var inlineDataValue)
            && inlineDataValue.ValueKind == JsonValueKind.String)
        {
            return inlineDataValue.GetString();
        }

        return null;
    }

    private static string TryExtractGoogleErrorMessage(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return string.Empty;
        }

        try
        {
            using var document = JsonDocument.Parse(responseBody);
            if (document.RootElement.TryGetProperty("error", out var errorElement)
                && errorElement.ValueKind == JsonValueKind.Object)
            {
                if (errorElement.TryGetProperty("message", out var messageElement)
                    && messageElement.ValueKind == JsonValueKind.String)
                {
                    return messageElement.GetString() ?? string.Empty;
                }

                if (errorElement.TryGetProperty("status", out var statusElement)
                    && statusElement.ValueKind == JsonValueKind.String)
                {
                    return statusElement.GetString() ?? string.Empty;
                }
            }
        }
        catch
        {
            // Keep empty on malformed body.
        }

        return string.Empty;
    }

    private sealed record GoogleGenerateContentResponse(bool Success, string Analysis, string ErrorMessage, System.Net.HttpStatusCode StatusCode)
    {
        public static GoogleGenerateContentResponse Success(string analysis)
            => new(true, analysis, string.Empty, System.Net.HttpStatusCode.OK);

        public static GoogleGenerateContentResponse Failure(System.Net.HttpStatusCode statusCode, string errorMessage)
            => new(false, string.Empty, errorMessage, statusCode);
    }
}
