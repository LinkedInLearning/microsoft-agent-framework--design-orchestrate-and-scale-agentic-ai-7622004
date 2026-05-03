using System.ComponentModel;

namespace AgentLab.Chapter1;

/// <summary>
/// Project context accumulated across conversation turns.
/// </summary>
public sealed record ProjectContext(
    string? ProductName,
    string? TechStack,
    [property: Description("The full feature specification in markdown. Always required — use empty string if none.")]
    string Feature = "")
{
    public static ProjectContext Empty => new(null, null, "");
}

/// <summary>
/// Delta extracted each turn by the LLM.
/// Null fields mean "no new info".
/// </summary>
public sealed record ProjectContextDelta(
    string? ProductName,
    string? TechStack,
    [property: Description("The full feature specification in markdown. Always required — use empty string if none.")]
    string Feature = "");
