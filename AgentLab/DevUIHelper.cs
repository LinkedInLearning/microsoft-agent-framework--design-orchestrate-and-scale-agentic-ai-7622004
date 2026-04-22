// SPDX-License-Identifier: LicenseRef-MAFPlayground-NPU-1.0-CH
// Copyright (c) 2025 Jose Luis Latorre

using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.DevUI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AgentLab;

/// <summary>
/// Helper class for running agents in DevUI mode.
/// Provides a consistent, DRY approach to launching any agent with the DevUI web interface.
/// </summary>
public static class DevUIHelper
{
    public const int DefaultPort = 5000;

    private static IChatClient CreateChatClient()
    {
        var azureClient = new AzureOpenAIClient(AIConfig.Endpoint, AIConfig.KeyCredential);
        return azureClient
            .GetChatClient(AIConfig.ModelDeployment)
            .AsIChatClient();
    }

    public static void RunWithDevUI(string agentName, string agentInstructions, int port = DefaultPort)
    {
        RunWithDevUI(new[] { new AgentSpec(agentName, agentInstructions) }, port);
    }

    public static void RunWithDevUI(AgentSpec spec, int port = DefaultPort)
    {
        RunWithDevUI(new[] { spec }, port);
    }

    public static void RunWithDevUI(IEnumerable<AgentSpec> agents, int port = DefaultPort)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development
        });

        var chatClient = CreateChatClient();
        builder.Services.AddChatClient(chatClient);

        Console.WriteLine("\n🤖 Registering agents:");
        var agentList = agents.ToList();
        foreach (var spec in agentList)
        {
            var agentBuilder = builder.AddAIAgent(spec.Name, spec.Instructions);
            if (spec.Tools?.Any() == true)
            {
                agentBuilder.WithAITools(spec.Tools.ToArray());
                Console.WriteLine($"   ✓ {spec.Name} (with {spec.Tools.Count()} tools)");
            }
            else
            {
                Console.WriteLine($"   ✓ {spec.Name}");
            }
        }

        builder.Services.AddOpenAIResponses();
        builder.Services.AddOpenAIConversations();

        var app = builder.Build();

        app.MapOpenAIResponses();
        app.MapOpenAIConversations();
        app.MapDevUI();

        PrintDevUIInfo($"http://localhost:{port}", agentList);

        app.Run($"http://localhost:{port}");
    }

    private static void PrintDevUIInfo(string url, List<AgentSpec> agents)
    {
        Console.WriteLine("\n" + new string('═', 80));
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✅ DevUI Server Started Successfully!");
        Console.ResetColor();
        Console.WriteLine(new string('═', 80));

        Console.WriteLine("\n📊 Available Endpoints:");
        Console.WriteLine($"   • DevUI Interface:           {url}/devui");
        Console.WriteLine($"   • OpenAI Responses API:      {url}/v1/responses");
        Console.WriteLine($"   • OpenAI Conversations API:  {url}/v1/conversations");

        Console.WriteLine("\n🤖 Registered Agents:");
        for (int i = 0; i < agents.Count; i++)
        {
            var toolInfo = agents[i].Tools?.Any() == true
                ? $" ({agents[i].Tools!.Count()} tools)"
                : "";
            Console.WriteLine($"   {i + 1}. {agents[i].Name}{toolInfo}");
        }

        Console.WriteLine("\n💡 How to Use:");
        Console.WriteLine($"   1. Open DevUI:            {url}/devui");
        Console.WriteLine("   2. Select an agent from the dropdown in DevUI");
        Console.WriteLine("   3. Send a message and see the response");

        Console.WriteLine("\n⚠️  Press Ctrl+C to stop the server");
        Console.WriteLine(new string('═', 80) + "\n");
    }
}

public class AgentSpec
{
    public AgentSpec(string name, string instructions)
    {
        Name = name;
        Instructions = instructions;
    }

    public AgentSpec(string name, string instructions, IEnumerable<AITool> tools)
    {
        Name = name;
        Instructions = instructions;
        Tools = tools;
    }

    public string Name { get; }
    public string Instructions { get; }
    public IEnumerable<AITool>? Tools { get; }
}
