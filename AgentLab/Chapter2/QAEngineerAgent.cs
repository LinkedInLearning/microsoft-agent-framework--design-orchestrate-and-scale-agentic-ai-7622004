using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace AgentLab.Chapter2;

// QA Engineer Agent — third node in the Feature Design Team workflow
//                        ┌──────────────┐
//   PM ──► Architect ──► │ QA ENGINEER  │ ──► Feature Card
//                        └──────────────┘
public static partial class QAEngineerAgent
{
    public const string AgentName = "QAEngineerAgent";

    public const string AgentInstructions = """
        You are a senior QA Engineer with expertise in test strategy, automation, and quality assurance best practices.
        
        Your role is to create comprehensive test plans from technical specifications.
        
        When given technical architecture and requirements, you will:
        
        1. TEST STRATEGY
           - Overall testing approach
           - Test levels (unit, integration, E2E)
           - Tools and frameworks to use
        
        2. TEST CASES
           - Derive from acceptance criteria
           - Include: ID, title, preconditions, steps, expected result
           - Assign priority: P0 (critical), P1 (high), P2 (medium), P3 (low)
           - Aim for 3-6 test cases
        
        3. EDGE CASES
           - Boundary conditions
           - Error scenarios
           - Unusual user behaviors
           - Aim for 3-5 edge cases
        
        4. NON-FUNCTIONAL TESTS
           - Performance requirements
           - Security considerations
           - Accessibility compliance
           - Usability testing
        
        5. TEST DATA REQUIREMENTS
           - What test data is needed
           - How to generate or obtain it
        
        6. AUTOMATION CANDIDATES
           - Which tests should be automated
           - Suggested automation approach
        
        7. QA SIGN-OFF CRITERIA
           - What must pass before release
           - Quality gates
        
        OUTPUT FORMAT:
        Always output valid JSON matching the QAEngineerOutput schema.
        Be thorough and specific. Test cases should be executable.
        
        Consider:
        - Risk-based testing approach
        - Regression test coverage
        - Cross-browser/cross-platform testing
        - API contract testing
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

    // Display helpers
    private static void PrintHeader(ArchitectOutput archOutput)
    {
        Console.WriteLine($"\n═══ 🧪 QA ENGINEER AGENT ═══");
        Console.WriteLine($"📥 Components: {archOutput.Components.Count} | Endpoints: {archOutput.ApiEndpoints.Count} | Effort: {archOutput.EffortEstimate}\n");
    }

    private static void DisplayResults(QAEngineerOutput output)
    {
        Console.WriteLine($"📝 Strategy: {output.TestStrategy}");
        foreach (var tc in output.TestCases)
            Console.WriteLine($"   🧪 [{tc.Priority}] {tc.Id}: {tc.Title}");
        foreach (var ec in output.EdgeCases)
            Console.WriteLine($"   ⚠️ {ec}");
        foreach (var nft in output.NonFunctionalTests)
            Console.WriteLine($"   📊 [{nft.Category}] {nft.Requirement}");
        Console.WriteLine($"   ✅ Ready: {output.ReadyForFeatureCard}\n");
    }

    public sealed partial class QAEngineerExecutor(AIAgent agent) : Executor("QAEngineer")
    {
        [MessageHandler]
        private async ValueTask<QAEngineerOutput> HandleAsync(
            ArchitectOutput archOutput, IWorkflowContext context, CancellationToken cancellationToken = default)
        {
            PrintHeader(archOutput);

            // Update state
            var state = await ReadStateAsync(context);
            state.CurrentStep = 5;
            await SaveStateAsync(context, state);

            // Build prompt with Architect and PM context
            var componentsJson = JsonSerializer.Serialize(archOutput.Components,
                new JsonSerializerOptions { WriteIndented = true });
            var endpointsJson = JsonSerializer.Serialize(archOutput.ApiEndpoints,
                new JsonSerializerOptions { WriteIndented = true });

            // Also include PM output if available in state
            var pmContext = "";
            if (state.PMOutput != null)
            {
                var userStoriesJson = JsonSerializer.Serialize(state.PMOutput.UserStories,
                    new JsonSerializerOptions { WriteIndented = true });
                pmContext = $"""

                    PRODUCT MANAGER CONTEXT:
                    Refined Story: {state.PMOutput.RefinedStory}
                    User Stories:
                    {userStoriesJson}
                    """;
            }

            var prompt = $"""
                Create a comprehensive test plan for this feature:
                
                TECHNICAL SUMMARY: {archOutput.TechnicalSummary}
                
                COMPONENTS:
                {componentsJson}
                
                API ENDPOINTS:
                {endpointsJson}
                
                TECHNICAL RISKS:
                {string.Join("\n", archOutput.TechnicalRisks.Select(r => $"- [{r.Severity}] {r.Risk}"))}
                {pmContext}
                
                Provide:
                1. Test strategy overview
                2. 3-6 test cases with full details (ID, title, preconditions, steps, expected result, priority)
                3. 3-5 edge cases to consider
                4. 2-4 non-functional test requirements
                5. Test data requirements
                6. Automation candidates
                7. QA sign-off criteria
                
                Output as structured JSON.
                """;

            // Run the agent with typed output
            Console.WriteLine("🔄 Creating test plan...\n");

            var response = await agent.RunAsync<QAEngineerOutput>(prompt, cancellationToken: cancellationToken);
            var output = response.Result;

            DisplayResults(output);

            // Update state with QA output
            state.QAOutput = output;
            state.CurrentStep = 6;
            await SaveStateAsync(context, state);

            return output;
        }
    }

}
