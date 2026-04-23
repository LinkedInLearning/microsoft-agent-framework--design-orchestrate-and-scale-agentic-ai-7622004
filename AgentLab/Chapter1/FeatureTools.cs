using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI;

namespace AgentLab.Chapter1;

/// <summary>
/// Shared local tools used by samples 03 (Tools) and 04 (MCP).
/// </summary>
public static class FeatureTools
{
        public static List<AITool> All =>
        [
            AIFunctionFactory.Create(StoryHealthCheck),
            AIFunctionFactory.Create(FeatureMetadata)
        ];
        

    [Description("Analyzes a user story for quality and completeness. Returns a health score 0-100.")]
    public static StoryHealthResult StoryHealthCheck(
        [Description("The user story or feature request text")] string storyText)
    {
        bool hasFormat = storyText.Contains("As a", StringComparison.OrdinalIgnoreCase)
                    && storyText.Contains("I want", StringComparison.OrdinalIgnoreCase);
        bool hasBenefit = storyText.Contains("so that", StringComparison.OrdinalIgnoreCase);
        int score = 40 + (hasFormat ? 30 : 0) + (hasBenefit ? 20 : 0)
                + (storyText.Split(' ').Length >= 10 ? 10 : 0);

        var result = new StoryHealthResult
        {
            HealthScore = Math.Min(100, score),
            QualityLevel = score >= 70 ? "High" : score >= 50 ? "Medium" : "Low",
            ImprovementSuggestions = !hasFormat
                ? ["Use 'As a [user], I want [goal] so that [benefit]' format"]
                : []
        };

        Console.WriteLine($"  🔧 StoryHealthCheck → {result.HealthScore}/100 ({result.QualityLevel})");
        return result;
    }
    
    [Description("Suggests components, priority, and complexity for a feature.")]
    public static FeatureMetadataResult FeatureMetadata(
        [Description("The feature description to analyze")] string featureDescription)
    {
        var desc = featureDescription.ToLowerInvariant();
        var components = new List<string>();

        if (desc.Contains("dashboard") || desc.Contains("chart") || desc.Contains("analytics")) components.Add("Analytics");
        if (desc.Contains("notification") || desc.Contains("alert")) components.Add("Notifications");
        if (desc.Contains("ui") || desc.Contains("theme") || desc.Contains("dark mode")) components.Add("UI/UX");
        if (desc.Contains("task") || desc.Contains("todo") || desc.Contains("assignment")) components.Add("TaskManagement");
        if (components.Count == 0) components.Add("Core");

        var priority = desc.Contains("critical") || desc.Contains("urgent") ? "High"
                    : desc.Contains("nice to have") || desc.Contains("minor") ? "Low"
                    : "Medium";

        var result = new FeatureMetadataResult
        {
            SuggestedComponents = components,
            SuggestedPriority = priority,
            EstimatedComplexity = components.Count > 2 ? "L" : components.Count > 1 ? "M" : "S"
        };

        Console.WriteLine($"  🔧 FeatureMetadata → [{string.Join(", ", components)}] Priority: {priority}");
        return result;
    }

}

// ── Result types ─────────────────────────────────────────────────────────────

public class StoryHealthResult
{
    [JsonPropertyName("healthScore")]
    [Description("Quality score from 0-100")]
    public int HealthScore { get; set; }

    [JsonPropertyName("qualityLevel")]
    [Description("Quality level: High, Medium, or Low")]
    public string QualityLevel { get; set; } = "Low";

    [JsonPropertyName("improvementSuggestions")]
    [Description("List of suggestions to improve the story")]
    public List<string> ImprovementSuggestions { get; set; } = [];
}

public class FeatureMetadataResult
{
    [JsonPropertyName("suggestedComponents")]
    [Description("Components/modules that should own this feature")]
    public List<string> SuggestedComponents { get; set; } = [];

    [JsonPropertyName("suggestedPriority")]
    [Description("Recommended priority: High, Medium, or Low")]
    public string SuggestedPriority { get; set; } = "Medium";

    [JsonPropertyName("estimatedComplexity")]
    [Description("T-Shirt size complexity: S, M, L, XL")]
    public string EstimatedComplexity { get; set; } = "M";
}
	