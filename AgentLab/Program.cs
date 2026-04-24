using AgentLab.Chapter1;   // uncommented in 01_02
// using AgentLab.Chapter2;   // uncommented in 02_03

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("╔═══════════════════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║                  MICROSOFT AGENT FRAMEWORK — COURSE                           ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════════════════════════╝");
Console.WriteLine();

while (true)
{
    Console.WriteLine("Select a sample to run:");
    Console.WriteLine();
    Console.WriteLine("  Chapter 1: Feature Planning Copilot");
    Console.WriteLine("  ─────────────────────────────────────");
    Console.WriteLine("  1. Sample 01 - Feature Planning Copilot (Basic Agent + DevUI)");
    Console.WriteLine("  2. Sample 02 - Feature Planning with Memory");
    Console.WriteLine("  3. Sample 03 - Feature Planning with Tools");
    Console.WriteLine("  4. Sample 04 - Feature Planning with MCP (GitHub)");
    Console.WriteLine();
    //Console.WriteLine("  Chapter 2: Feature Design Team (Workflows)");
    //Console.WriteLine("  ─────────────────────────────────────────────");
    //Console.WriteLine("  5. Sample 03 - Feature Design Team (Mock Skeleton)");
    //Console.WriteLine("  6. Sample 04-07 - Feature Design Team (Full AI Workflow)");
    //Console.WriteLine();
    Console.WriteLine("  q. Quit");
    Console.WriteLine();
    Console.Write("Enter your choice: ");

    var input = Console.ReadLine()?.Trim().ToLowerInvariant();

    if (string.IsNullOrEmpty(input))
        continue;

    if (input == "q" || input == "quit")
    {
        Console.WriteLine("\n👋 Goodbye!");
        break;
    }

    Console.WriteLine();

    try
    {
        switch (input)
        {
            case "1":   // uncommented in 01_02
              await FeaturePlanningCopilot.Run();
              break;
            case "2":
               await FeaturePlanningWithMemory.RunAsync();
               break;
            case "3":
               await FeaturePlanningWithTools.RunAsync();
               break;
            case "4":
               await FeaturePlanningWithMCP.RunAsync();
               break;
            //case "5":
            //    await FeatureWorkflow.ExecuteAsync(isMock: true);
            //    break;
            //case "6":
            //    await FeatureWorkflow.ExecuteAsync(isMock: false);
            //    break;
            default:
                Console.WriteLine("❌ Invalid choice. Please try again.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n❌ Error: {ex.Message}");
        Console.ResetColor();
    }

    Console.WriteLine("\nPress any key to continue...");
    Console.ReadKey(true);
    Console.Clear();
}
