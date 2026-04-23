using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentLab.Chapter1;

public static class FeaturePlanningCopilot
{
    public static async Task Run()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Step 1: Connect to Azure OpenAI
        var azureClient = new AzureOpenAIClient(AIConfig.Endpoint, AIConfig.KeyCredential);
        IChatClient chatClient = azureClient
            .GetChatClient(AIConfig.ModelDeployment)
                .AsIChatClient();

        // Step 2: Create the agent — just a chat client + a system prompt
        AIAgent agent = chatClient.AsAIAgent(
            name: AgentInstructions.AgentName,
            instructions: AgentInstructions.CoreInstructions);
        
        // Step 3: Run the agent programmatically
        string featureIdea = "Users should be able to export their data as PDF";
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n🧑 Input: \"{featureIdea}\"\n");
        Console.ResetColor();

        AgentSession session = await agent.CreateSessionAsync(); // fresh conversation, no prior context
        var response = await agent.RunAsync(featureIdea, session);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"🤖 Agent: {response.Text}");
        Console.ResetColor();

        // Step 4: Launch DevUI for interactive exploration
        Console.WriteLine("\n✅ Now launching DevUI — try your own inputs at http://localhost:5000/devui\n");
        DevUIHelper.RunWithDevUI(AgentInstructions.AgentName, AgentInstructions.CoreInstructions);
    }
}

                    