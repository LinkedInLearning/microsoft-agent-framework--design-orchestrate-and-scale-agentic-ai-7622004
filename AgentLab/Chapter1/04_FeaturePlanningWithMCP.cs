using System.Text;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace AgentLab.Chapter1;

public static class FeaturePlanningWithMCP
{
    public static async Task RunAsync()
    {
        Console.OutputEncoding = Encoding.UTF8;

        // Step 1: Connect to Azure OpenAI (same as 03)
        var azureClient = new AzureOpenAIClient(AIConfig.Endpoint, AIConfig.KeyCredential);
        IChatClient chatClient = azureClient
            .GetChatClient(AIConfig.ModelDeployment)
            .AsIChatClient();

        // Step 2: Connect to GitHub MCP server (NEW — requires Node.js + GITHUB_PERSONAL_ACCESS_TOKEN)
        var githubToken = Environment.GetEnvironmentVariable("GITHUB_PERSONAL_ACCESS_TOKEN")
            ?? throw new InvalidOperationException("Set GITHUB_PERSONAL_ACCESS_TOKEN env var with 'repo' scope.");

        var mcpClient = await McpClient.CreateAsync(new StdioClientTransport(new()
        {
            Name = "GitHubMCPServer",
            Command = "npx",
            Arguments = ["-y", "@modelcontextprotocol/server-github"],
            EnvironmentVariables = new Dictionary<string, string?>
            {
                ["GITHUB_PERSONAL_ACCESS_TOKEN"] = githubToken
            }
        }));

        // Step 3: Combine local tools + MCP tools (NEW — dynamic tool discovery)
        var localTools = FeatureTools.All;

        var mcpTools = await mcpClient.ListToolsAsync();
        var allTools = localTools.Concat(mcpTools).ToList();
        Console.WriteLine($"  🔌 MCP connected — {mcpTools.Count} GitHub tools discovered");

        // Step 4: Create agent — memory + tools + MCP (accumulative)
        var memory = new ProjectContextMemory(chatClient);

        AIAgent agent = chatClient.AsAIAgent(new ChatClientAgentOptions
        {
            Name = AgentInstructions.AgentName,
            ChatOptions = new()
            {
                Instructions = AgentInstructions.Compose(
                    AgentInstructions.WithProjectContext,
                    AgentInstructions.WithTools,
                    AgentInstructions.WithMCP),
                Tools = allTools
            },
            AIContextProviders = [memory]
        });

        // Step 5: Create session and print welcome
        AgentSession session = await agent.CreateSessionAsync();

        Console.WriteLine("✅ DEMO4 — Feature Planning Copilot with Memory + Tools + MCP (GitHub)");
        Console.WriteLine("Provide context, then ask for features. Try: 'Create a GitHub issue in owner/repo'");
        Console.WriteLine("Type /exit to quit.\n");

        // Step 6: Interactive loop with streaming (same shape as 03)
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

        await mcpClient.DisposeAsync();




    }
}
                    