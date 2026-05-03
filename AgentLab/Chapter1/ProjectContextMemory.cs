using System.Text;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentLab.Chapter1;

/// <summary>
/// Memory provider that accumulates project facts across conversation turns.
/// ProvideAIContextAsync  → injects current context before each agent run.
/// StoreAIContextAsync    → extracts new facts via a side-LLM call after each run.
/// </summary>
internal sealed class ProjectContextMemory : AIContextProvider
{
    private readonly IChatClient _chatClient;
    public ProjectContext Context { get; private set; }

    /// <summary>
    /// Creates a new ProjectContextMemory with empty initial state.
    /// State accumulates across turns via ProvideAIContextAsync / StoreAIContextAsync.
    /// </summary>
    public ProjectContextMemory(IChatClient chatClient)
    {
        _chatClient = chatClient;
        Context = ProjectContext.Empty;
    }

    // ── Before each turn: inject what we know (and flag what's missing) ──
    protected override ValueTask<AIContext> ProvideAIContextAsync(InvokingContext context, CancellationToken ct = default)
    {
        var sb = new StringBuilder();
        sb.AppendLine("## Project Context (authoritative)");
        if (!string.IsNullOrWhiteSpace(Context.ProductName)) sb.AppendLine($"- Product: {Context.ProductName}");
        if (!string.IsNullOrWhiteSpace(Context.TechStack)) sb.AppendLine($"- Tech stack: {Context.TechStack}");
        if (!string.IsNullOrWhiteSpace(Context.Feature))
        {
            sb.AppendLine();
            sb.AppendLine("## Feature Specification (work-in-progress)");
            sb.AppendLine(Context.Feature);
        }

        var missing = new List<string>();
        if (string.IsNullOrWhiteSpace(Context.ProductName)) missing.Add("ProductName");
        if (string.IsNullOrWhiteSpace(Context.TechStack)) missing.Add("TechStack");
        if (string.IsNullOrWhiteSpace(Context.Feature)) missing.Add("Feature");

        if (missing.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine($"## Missing context — ask the user about: {string.Join(", ", missing)}");
        }

        return new ValueTask<AIContext>(new AIContext { Instructions = sb.ToString() });
    }

// ── After each turn: extract facts via a side-LLM call ──
protected override async ValueTask StoreAIContextAsync(InvokedContext context, CancellationToken ct = default)
{
    if (!context.RequestMessages.Any(m => m.Role == ChatRole.User)) return;

    var allMessages = context.RequestMessages
        .Concat(context.ResponseMessages ?? [])
        .ToList();

    var extraction = await _chatClient.GetResponseAsync<ProjectContextDelta>(
        allMessages,
        new ChatOptions
        {
            Instructions = """
                You are a precise information extraction assistant.
                Extract structured data from the conversation messages.

                - ProductName: the name of the product/project
                - TechStack: technologies, languages, frameworks mentioned
                - Feature: the full feature specification in markdown.
                  If the ASSISTANT's response contains a spec between
                  <<<FEATURE_SPEC>>> and <<<END_FEATURE_SPEC>>> delimiters,
                  copy verbatim (excluding delimiter lines).
                  Otherwise build it from the conversation or use empty string.
                  Feature must NEVER be null.
                """
        },
        cancellationToken: ct);

    var delta = extraction.Result;

    Context = Context with
    {
        ProductName = delta.ProductName ?? Context.ProductName,
        TechStack   = delta.TechStack   ?? Context.TechStack,
        Feature     = !string.IsNullOrWhiteSpace(delta.Feature) ? delta.Feature : Context.Feature
    };
}


}
