using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace AgentLab.Chapter2;

// Architect Agent — second node in the Feature Design Team workflow
//          ┌──────────────┐
//   PM ──► │  ARCHITECT   │ ──► QA ──► Feature Card
//          └──────────────┘
public static partial class ArchitectAgent
{
    public const string AgentName = "ArchitectAgent";

    public const string AgentInstructions = """
        You are a senior Software Architect with expertise in system design, cloud architecture, and modern development practices.
        
        Your role is to transform product requirements into technical specifications.
        
        When given user stories and product requirements, you will:
        
        1. TECHNICAL SUMMARY
           - High-level approach overview
           - Key architectural decisions
           - Technology choices and justifications
        
        2. COMPONENT BREAKDOWN
           - Identify required components/modules
           - Define responsibilities for each
           - Specify interfaces between components
           - Component types: Service, UI, Database, API, Worker, etc.
        
        3. DATA MODEL
           - Define data entities
           - Specify properties for each entity
           - Document relationships between entities
        
        4. API DESIGN (if applicable)
           - Define endpoints (method, path, description)
           - Specify request/response formats
           - Note authentication requirements
        
        5. DEPENDENCIES
           - External services or APIs
           - Third-party libraries
           - Infrastructure requirements
        
        6. TECHNICAL RISKS
           - Identify potential risks
           - Assess severity (High/Medium/Low)
           - Propose mitigations
        
        7. EFFORT ESTIMATE
           - T-shirt size: XS, S, M, L, XL
           - Brief justification
        
        OUTPUT FORMAT:
        Always output valid JSON matching the ArchitectOutput schema.
        Be specific and implementation-ready. Include concrete details.
        
        Consider:
        - Scalability and performance
        - Security and data privacy
        - Maintainability and testing
        - Integration with existing systems
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
    private static void PrintHeader(ProductManagerOutput pmOutput)
    {
        Console.WriteLine($"\n═══ 🏗️ ARCHITECT AGENT ═══");
        Console.WriteLine($"📥 Story: \"{pmOutput.RefinedStory}\" | Stories: {pmOutput.UserStories.Count} | Priority: {pmOutput.Priority}\n");
    }

    private static void DisplayResults(ArchitectOutput output)
    {
        Console.WriteLine($"📝 Summary: {output.TechnicalSummary}");
        foreach (var c in output.Components)
            Console.WriteLine($"   🧩 {c.Name} ({c.Type}): {c.Responsibility}");
        foreach (var e in output.DataModel)
            Console.WriteLine($"   📊 {e.Name}: {string.Join(", ", e.Properties.Take(5))}");
        foreach (var ep in output.ApiEndpoints)
            Console.WriteLine($"   🌐 {ep.Method} {ep.Path}: {ep.Description}");
        foreach (var r in output.TechnicalRisks)
            Console.WriteLine($"   ⚠️ [{r.Severity}] {r.Risk}");
        Console.WriteLine($"   📏 Effort: {output.EffortEstimate} | ✅ Ready: {output.ReadyForQA}\n");
    }

    public sealed partial class ArchitectExecutor(AIAgent agent) : Executor("Architect")
    {
        [MessageHandler]
        private async ValueTask<ArchitectOutput> HandleAsync(
        ProductManagerOutput pmOutput, IWorkflowContext context, CancellationToken cancellationToken = default)
        {
            PrintHeader(pmOutput);

            // Update state
            var state = await ReadStateAsync(context);
            state.CurrentStep = 3;
            await SaveStateAsync(context, state);

            // Build prompt with PM context
            var userStoriesJson = JsonSerializer.Serialize(pmOutput.UserStories,
                new JsonSerializerOptions { WriteIndented = true });

            var prompt = $"""
                Create a technical architecture for this feature:
                
                REFINED STORY: {pmOutput.RefinedStory}
                
                USER STORIES:
                {userStoriesJson}
                
                SUCCESS METRICS: {string.Join(", ", pmOutput.SuccessMetrics)}
                
                SCOPE NOTES: {pmOutput.ScopeNotes}
                
                Provide:
                1. Technical summary with approach and key decisions
                2. Component breakdown (2-4 components)
                3. Data model (1-3 entities)
                4. API endpoints (if applicable, 2-4 endpoints)
                5. Dependencies (external services, libraries)
                6. Technical risks with mitigations
                7. Effort estimate (T-shirt size with justification)
                
                Output as structured JSON.
                """;

            // Run the agent with typed output
            Console.WriteLine("🔄 Designing technical architecture...\n");

            var response = await agent.RunAsync<ArchitectOutput>(prompt, cancellationToken: cancellationToken);
            var output = response.Result;

            DisplayResults(output);

            // Update state with Architect output
            state.ArchitectOutput = output;
            state.CurrentStep = 4;
            await SaveStateAsync(context, state);

            return output;
        }
    }


}
