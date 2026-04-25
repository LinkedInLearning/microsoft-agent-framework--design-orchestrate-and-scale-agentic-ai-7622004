using Azure.AI.OpenAI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace AgentLab.Chapter2;

// Feature Design Team — Workflow orchestrator
// string ──► PM ──► Architect ──► QA ──► FeatureCard
public static class FeatureWorkflow
{
    private const string SampleFeatureIdea = "add dark mode support";

    // Runs the workflow: isMock=true for mock executors, false for real AI agents
    public static async Task ExecuteAsync(bool isMock = false)
    {
        var chatClient = isMock ? null : CreateChatClient();

        Executor productManagerExecutor = isMock
            ? new FeatureDesignTeam.ProductManagerMockExecutor()
            : new ProductManagerAgent.ProductManagerExecutor(ProductManagerAgent.CreateAgent(chatClient!));

        Executor architectExecutor = isMock
            ? new FeatureDesignTeam.ArchitectMockExecutor()
            : new ArchitectAgent.ArchitectExecutor(ArchitectAgent.CreateAgent(chatClient!));

        Executor qaEngineerExecutor = isMock
            ? new FeatureDesignTeam.QAEngineerMockExecutor()
            : new QAEngineerAgent.QAEngineerExecutor(QAEngineerAgent.CreateAgent(chatClient!));

        // Placeholder mocks — will be replaced with real executors in upcoming videos
        Executor featureCardExecutor = new FeatureDesignTeam.FeatureCardMockExecutor();

        // ── Build & run the workflow ──
        var workflow = new WorkflowBuilder(productManagerExecutor)
            .AddEdge(productManagerExecutor, architectExecutor)
            .AddEdge(architectExecutor, qaEngineerExecutor)
            .AddEdge(qaEngineerExecutor, featureCardExecutor)
            .WithOutputFrom(featureCardExecutor)
            .WithName("feature-design-team")
            .Build();

        // Visualize the workflow graph
        Console.WriteLine("\n📊 Workflow Graph (Mermaid):");
        Console.WriteLine(workflow.ToMermaidString());
        Console.WriteLine();

        var title = isMock
            ? "CHAPTER 2 — FEATURE DESIGN TEAM (MOCK SKELETON)"
            : "CHAPTER 2 — FEATURE DESIGN TEAM (COMPLETE)";

        await RunWorkflowAsync(title, workflow, SampleFeatureIdea);
    }

    // Streams workflow events and displays the final FeatureCard
    private static async Task RunWorkflowAsync(string title, Workflow workflow, string featureIdea)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine($"\n{new string('═', 70)}");
        Console.WriteLine($"   {title}");
        Console.WriteLine($"{new string('═', 70)}\n");

        Console.WriteLine($"📝 Feature Idea: \"{featureIdea}\"");
        Console.WriteLine(new string('─', 60));

        await using StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, featureIdea);

        FeatureCard? finalCard = null;

        await foreach (WorkflowEvent evt in run.WatchStreamAsync())
        {
            switch (evt)
            {
                case ExecutorInvokedEvent invoked:
                    Console.WriteLine($"  ▶ {invoked.ExecutorId} started");
                    break;
                case ExecutorCompletedEvent completed:
                    Console.WriteLine($"  ✔ {completed.ExecutorId} completed");
                    break;
                case ExecutorFailedEvent failed:
                    Console.WriteLine($"  ✖ {failed.ExecutorId} failed: {failed.Data?.Message}");
                    break;
                case WorkflowOutputEvent output when output.Data is FeatureCard card:
                    finalCard = card;
                    break;
            }
        }

        if (finalCard != null)
        {
            DisplayFeatureCard(finalCard);
        }

        Console.WriteLine($"\n{new string('═', 70)}");
        Console.WriteLine("✅ Workflow complete!");
        Console.WriteLine(new string('═', 70));
    }

    private static IChatClient CreateChatClient()
    {
        var azureClient = new AzureOpenAIClient(AIConfig.Endpoint, AIConfig.KeyCredential);
        return azureClient.GetChatClient(AIConfig.ModelDeployment).AsIChatClient();
    }

    private static void DisplayFeatureCard(FeatureCard card)
    {
        Console.WriteLine(new string('═', 70));
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("📋 FEATURE CARD");
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

        if (!string.IsNullOrEmpty(card.Summary))
        {
            Console.WriteLine($"  📝 Summary: {card.Summary}");
            Console.WriteLine();
        }

        if (card.UserStories.Count > 0)
        {
            Console.WriteLine($"  📖 User Stories ({card.UserStories.Count}):");
            foreach (var story in card.UserStories)
                Console.WriteLine($"     • As a {story.AsA}, I want {story.IWant} so that {story.SoThat}");
            Console.WriteLine();
        }

        if (card.Components.Count > 0)
        {
            Console.WriteLine($"  🏗️  Components ({card.Components.Count}):");
            foreach (var comp in card.Components)
                Console.WriteLine($"     • {comp.Name} ({comp.Type})");
            Console.WriteLine();
        }

        if (card.TestCases.Count > 0)
        {
            Console.WriteLine($"  🧪 Test Cases ({card.TestCases.Count}):");
            foreach (var tc in card.TestCases)
                Console.WriteLine($"     • [{tc.Priority}] {tc.Id}: {tc.Title}");
            Console.WriteLine();
        }

        if (card.QASignOffCriteria.Count > 0)
        {
            Console.WriteLine($"  ✅ QA Sign-off ({card.QASignOffCriteria.Count}):");
            foreach (var c in card.QASignOffCriteria)
                Console.WriteLine($"     • {c}");
            Console.WriteLine();
        }
    }

}
                    