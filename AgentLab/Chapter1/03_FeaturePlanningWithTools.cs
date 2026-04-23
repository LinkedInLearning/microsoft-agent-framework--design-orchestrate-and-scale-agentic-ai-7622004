using System.Text;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentLab.Chapter1;

public static class FeaturePlanningWithTools
{
    public static async Task RunAsync()
    {
        Console.OutputEncoding = Encoding.UTF8;

        // Step 1: Connect to Azure OpenAI and define two tools
        var azureClient = new AzureOpenAIClient(AIConfig.Endpoint, AIConfig.KeyCredential);
        IChatClient chatClient = azureClient
            .GetChatClient(AIConfig.ModelDeployment)
            .AsIChatClient();

        var tools = FeatureTools.All;

        // Step 2: Create the agent — memory from 02 + tools (new)
        var memory = new ProjectContextMemory(chatClient);

        AIAgent agent = chatClient.AsAIAgent(new ChatClientAgentOptions
        {
            Name = AgentInstructions.AgentName,
            ChatOptions = new()
            {
                Instructions = AgentInstructions.Compose(AgentInstructions.WithProjectContext, AgentInstructions.WithTools),
                Tools = tools
            },
            AIContextProviders = [memory]
        });

        // Step 3: Create session and print welcome
        AgentSession session = await agent.CreateSessionAsync();

        Console.WriteLine("✅ DEMO3 — Feature Planning Copilot with Memory + Tools");
        Console.WriteLine("Seed context like: 'Our product is TaskFlow, built with .NET 9 and Blazor'");
        Console.WriteLine("Then ask for features. Type /exit to quit.\n");

        // Step 4: Interactive loop with streaming (builds on 02, adding tools)
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("🧑 You: ");
            Console.ResetColor();
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) continue;
            if (input.Equals("/exit", StringComparison.OrdinalIgnoreCase)) break;

            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("🤖 Agent: ");
                await foreach (var update in agent.RunStreamingAsync(input, session))
                {
                    if (!string.IsNullOrEmpty(update.Text))
                        Console.Write(update.Text);
                }
                Console.ResetColor();
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n⚠️ Agent error: {ex.Message}");
            }
        }


    }
}
                    