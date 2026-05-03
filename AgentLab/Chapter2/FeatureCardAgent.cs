using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace AgentLab.Chapter2;

// Feature Card Agent — final node, aggregates all outputs into a Feature Card
//                            ┌──────────────┐
//   PM ──► Architect ──► QA ──► │ FEATURE CARD │
//                            └──────────────┘
public static partial class FeatureCardAgent
{
    public const string AgentName = "FeatureCardAgent";

    public const string AgentInstructions = """
        You are a senior Technical Writer and Product Analyst who synthesizes
        multi-perspective analysis into concise, actionable feature cards.
        
        You will receive product requirements, technical architecture, and QA analysis
        for a feature. Your job is to create a unified Feature Card that:
        
        1. TITLE — Clear, concise feature title (5-8 words)
        2. SUMMARY — One paragraph executive summary combining all perspectives
        3. USER STORIES — From the PM analysis
        4. SUCCESS METRICS — From the PM analysis
        5. PRIORITY — From the PM analysis
        6. TECHNICAL SUMMARY — From the Architect analysis
        7. COMPONENTS — From the Architect analysis
        8. EFFORT ESTIMATE — From the Architect analysis
        9. TECHNICAL RISKS — From the Architect analysis
        10. ACCEPTANCE CRITERIA — Derived from user stories and QA sign-off
        11. TEST CASES — From the QA analysis
        12. QA SIGN-OFF CRITERIA — From the QA analysis
        
        Generate a unique feature_id in format FEAT-YYYYMMDD-NNNN.
        Set status to "Ready".
        
        OUTPUT FORMAT: Valid JSON matching the FeatureCard schema.
        Be thorough yet concise. Every field should have meaningful content.
        """;


    public static AIAgent CreateAgent(IChatClient chatClient)
    {
        return chatClient.AsAIAgent(
            name: AgentName,
            instructions: AgentInstructions);
    }

    private static async Task<FeatureWorkflowState> ReadStateAsync(IWorkflowContext context)
    {
        var state = await context.ReadStateAsync<FeatureWorkflowState>(
            FeatureStateShared.Key, scopeName: FeatureStateShared.Scope);
        return state ?? new FeatureWorkflowState();
    }

    private static ValueTask SaveStateAsync(IWorkflowContext context, FeatureWorkflowState state)
        => context.QueueStateUpdateAsync(FeatureStateShared.Key, state, scopeName: FeatureStateShared.Scope);

    public sealed partial class FeatureCardExecutor(AIAgent agent) : Executor("FeatureCard")
    {
        [MessageHandler]
        private async ValueTask<FeatureCard> HandleAsync(
            QAEngineerOutput qaOutput, IWorkflowContext context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"\n═══ 📦 FEATURE CARD AGENT ═══");

            var state = await ReadStateAsync(context);
            var pmOutput = state.PMOutput;
            var archOutput = state.ArchitectOutput;

            Console.WriteLine($"\n📥 Aggregating all outputs:");
            Console.WriteLine($"   PM: {pmOutput?.UserStories.Count ?? 0} stories, Priority: {pmOutput?.Priority ?? "?"}");
            Console.WriteLine($"   Architect: {archOutput?.Components.Count ?? 0} components, Effort: {archOutput?.EffortEstimate ?? "?"}");
            Console.WriteLine($"   QA: {qaOutput.TestCases.Count} test cases, {qaOutput.EdgeCases.Count} edge cases");
            Console.WriteLine();

            // Build comprehensive prompt with all inputs
            var serializerOptions = new JsonSerializerOptions { WriteIndented = true };
            var pmJson = JsonSerializer.Serialize(pmOutput, serializerOptions);
            var archJson = JsonSerializer.Serialize(archOutput, serializerOptions);
            var qaJson = JsonSerializer.Serialize(qaOutput, serializerOptions);

            var prompt = $"""
                Create a comprehensive Feature Card by synthesizing these analyses:

                ORIGINAL IDEA: {state.OriginalIdea}

                PRODUCT MANAGER ANALYSIS:
                {pmJson}

                ARCHITECT ANALYSIS:
                {archJson}

                QA ENGINEER ANALYSIS:
                {qaJson}

                Produce a unified Feature Card with all fields populated.
                """;

            Console.WriteLine("🔄 Generating Feature Card...\n");

            var response = await agent.RunAsync<FeatureCard>(prompt, cancellationToken: cancellationToken);
            var card = response.Result;

            // Update state
            state.FeatureCard = card;
            state.CurrentStep = 4;
            state.Status = WorkflowStatus.Complete;
            await SaveStateAsync(context, state);

            // Display
            DisplayCard(card);

            return card;
        }

        private static void DisplayCard(FeatureCard card)
        {
            Console.WriteLine(new string('═', 70));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("📋 FEATURE CARD — COMPLETE SPECIFICATION");
            Console.ResetColor();
            Console.WriteLine(new string('═', 70));
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  Feature ID: {card.FeatureId}");
            Console.WriteLine($"  Title:      {card.Title}");
            Console.WriteLine($"  Priority:   {card.Priority}");
            Console.WriteLine($"  Effort:     {card.EffortEstimate}");
            Console.WriteLine($"  Status:     {card.Status}");
            Console.ResetColor();
            Console.WriteLine();

            Console.WriteLine("  📝 SUMMARY");
            Console.WriteLine($"  {new string('-', 40)}");
            Console.WriteLine($"  {card.Summary}");
            Console.WriteLine();

            Console.WriteLine($"  📖 USER STORIES ({card.UserStories.Count})");
            Console.WriteLine($"  {new string('-', 40)}");
            foreach (var story in card.UserStories)
            {
                Console.WriteLine($"    As a {story.AsA}");
                Console.WriteLine($"    I want {story.IWant}");
                Console.WriteLine($"    So that {story.SoThat}");
                Console.WriteLine();
            }

            Console.WriteLine($"  📊 SUCCESS METRICS ({card.SuccessMetrics.Count})");
            Console.WriteLine($"  {new string('-', 40)}");
            foreach (var metric in card.SuccessMetrics)
                Console.WriteLine($"    • {metric}");
            Console.WriteLine();

            Console.WriteLine("  🏗️  TECHNICAL DESIGN");
            Console.WriteLine($"  {new string('-', 40)}");
            Console.WriteLine($"    {card.TechnicalSummary}");
            Console.WriteLine();
            foreach (var comp in card.Components)
                Console.WriteLine($"    • {comp.Name} ({comp.Type})");
            Console.WriteLine();

            if (card.TechnicalRisks.Count > 0)
            {
                Console.WriteLine($"  ⚠️  RISKS ({card.TechnicalRisks.Count})");
                Console.WriteLine($"  {new string('-', 40)}");
                foreach (var risk in card.TechnicalRisks)
                    Console.WriteLine($"    [{risk.Severity}] {risk.Risk}");
                Console.WriteLine();
            }

            Console.WriteLine($"  🧪 TEST CASES ({card.TestCases.Count})");
            Console.WriteLine($"  {new string('-', 40)}");
            foreach (var tc in card.TestCases)
                Console.WriteLine($"    [{tc.Priority}] {tc.Id}: {tc.Title}");
            Console.WriteLine();

            Console.WriteLine($"  ✅ QA SIGN-OFF ({card.QASignOffCriteria.Count})");
            Console.WriteLine($"  {new string('-', 40)}");
            foreach (var criteria in card.QASignOffCriteria)
                Console.WriteLine($"    ☐ {criteria}");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  ✅ Feature Card generation complete!");
            Console.ResetColor();
            Console.WriteLine(new string('═', 70));
        }
    }

}
