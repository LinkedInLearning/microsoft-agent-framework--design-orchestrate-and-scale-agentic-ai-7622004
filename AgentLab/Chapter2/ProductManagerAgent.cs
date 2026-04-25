using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace AgentLab.Chapter2;

// Product Manager Agent — first node in the Feature Design Team workflow
//   ┌──────────────────┐
//   │ PRODUCT MANAGER  │ ──► Architect ──► QA ──► Feature Card
//   └──────────────────┘
public static partial class ProductManagerAgent
{
    public const string AgentName = "ProductManagerAgent";

    public const string AgentInstructions = """
        You are a senior Product Manager with expertise in user-centered design and agile methodologies.
        
        Your role is to transform rough feature ideas into well-structured product specifications.
        
        When given a feature idea, you will:
        
        1. REFINE THE STORY
           - Clarify the value proposition
           - Identify what problem this solves
           - Make the benefit concrete and measurable
        
        2. IDENTIFY USER PERSONAS
           - Who benefits from this feature?
           - What are their characteristics?
           - What are their needs and pain points?
        
        3. CREATE USER STORIES
           - Format: "As a [persona], I want [goal] so that [benefit]"
           - Each story should be independent and testable
           - Include 3-5 acceptance criteria per story
        
        4. DEFINE SUCCESS METRICS
           - How will we measure success?
           - What KPIs should we track?
           - What's the target for each metric?
        
        5. ASSESS PRIORITY & SCOPE
           - Priority: High (critical), Medium (important), Low (nice-to-have)
           - Note what's in scope vs. out of scope
           - Identify dependencies or prerequisites
        
        OUTPUT FORMAT:
        Always output valid JSON matching the ProductManagerOutput schema.
        Be specific and actionable. Avoid vague statements.
        
        Example output structure:
        {
          "refined_story": "Clear value proposition statement",
          "user_personas": [...],
          "user_stories": [...],
          "success_metrics": [...],
          "priority": "Medium",
          "scope_notes": "In scope: X, Y. Out of scope: Z",
          "ready_for_architecture": true
        }
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
    private static void PrintHeader(string featureIdea)
    {
        Console.WriteLine($"\n═══ 👔 PRODUCT MANAGER AGENT ═══");
        Console.WriteLine($"📥 Input: \"{featureIdea}\"\n");
    }

    private static void DisplayResults(ProductManagerOutput output)
    {
        Console.WriteLine($"📝 Story: {output.RefinedStory}");
        foreach (var p in output.UserPersonas)
            Console.WriteLine($"   👤 {p.Name}: {p.Description}");
        foreach (var s in output.UserStories)
            Console.WriteLine($"   📖 As a {s.AsA}, I want {s.IWant} so that {s.SoThat}");
        foreach (var m in output.SuccessMetrics)
            Console.WriteLine($"   📊 {m}");
        Console.WriteLine($"   ⚡ Priority: {output.Priority} | ✅ Ready: {output.ReadyForArchitecture}\n");
    }

    public sealed partial class ProductManagerExecutor(AIAgent agent) : Executor("ProductManager")
    {
        [MessageHandler]
        private async ValueTask<ProductManagerOutput> HandleAsync(
            string featureIdea, IWorkflowContext context, CancellationToken cancellationToken = default)
        {
            PrintHeader(featureIdea);

            // Update state
            var state = await ReadStateAsync(context);
            state.OriginalIdea = featureIdea;
            state.CurrentStep = 1;
            state.Status = WorkflowStatus.InProgress;
            await SaveStateAsync(context, state);

            // Build prompt with context
            var prompt = $"""
                Analyze this feature idea and produce a comprehensive PM specification:
                
                FEATURE IDEA: {featureIdea}
                
                Provide:
                1. A refined story with clear value proposition
                2. 2-3 user personas who would benefit
                3. 1-3 user stories with acceptance criteria
                4. 2-4 success metrics
                5. Priority assessment (High/Medium/Low)
                6. Scope notes (what's in/out)
                
                Output as structured JSON.
                """;

            // Run the agent with typed output
            Console.WriteLine("🔄 Analyzing feature idea...\n");

            var response = await agent.RunAsync<ProductManagerOutput>(prompt, cancellationToken: cancellationToken);
            var output = response.Result;

            DisplayResults(output);

            // Update state with PM output
            state.PMOutput = output;
            state.CurrentStep = 2;
            await SaveStateAsync(context, state);

            return output;
        }
    }


}
