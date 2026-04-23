using System.Text;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentLab.Chapter1;

public static class FeaturePlanningWithMemory
{
    public static async Task RunAsync()
    {
        Console.OutputEncoding = Encoding.UTF8;

        // Step 1: Create agent with memory via AIContextProviders
        var azureClient = new AzureOpenAIClient(AIConfig.Endpoint, AIConfig.KeyCredential);
        IChatClient chatClient = azureClient
            .GetChatClient(AIConfig.ModelDeployment)
            .AsIChatClient();

        var memory = new ProjectContextMemory(chatClient);

        AIAgent agent = chatClient.AsAIAgent(new ChatClientAgentOptions
        {
            Name = AgentInstructions.AgentName,
            ChatOptions = new() { Instructions = AgentInstructions.Compose(AgentInstructions.WithProjectContext) },
            AIContextProviders = [memory]
        });

        // Step 2: Create a session — multi-turn state + memory live here
        AgentSession session = await agent.CreateSessionAsync();

        Console.WriteLine("✅ DEMO2 — Feature Planning Copilot with Project Memory");
        Console.WriteLine("Seed context like: 'Our product is TaskFlow, built with .NET 9 and Blazor'");
        Console.WriteLine("Then ask for features. Type /exit to quit.\n");

        // Step 3: Interactive loop with streaming
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
                    