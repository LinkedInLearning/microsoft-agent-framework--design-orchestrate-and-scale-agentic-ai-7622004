using Microsoft.Agents.AI.Workflows;

namespace AgentLab.Chapter2;

// Mock executors — validate workflow structure before wiring AI agents
public static partial class FeatureDesignTeam
{
    internal sealed partial class ProductManagerMockExecutor() : Executor("ProductManager")
    {
        [MessageHandler]
        private ValueTask<ProductManagerOutput> HandleAsync(
            string featureIdea, IWorkflowContext context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"\n👔 Product Manager: received \"{featureIdea}\"");

            return ValueTask.FromResult(new ProductManagerOutput
            {
                RefinedStory = $"[PM would refine: {featureIdea}]",
                ReadyForArchitecture = true
            });
        }
    }

    internal sealed partial class ArchitectMockExecutor() : Executor("Architect")
    {
        [MessageHandler]
        private ValueTask<ArchitectOutput> HandleAsync(
            ProductManagerOutput pmOutput, IWorkflowContext context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"🏗️ Architect: received \"{pmOutput.RefinedStory}\"");

            return ValueTask.FromResult(new ArchitectOutput
            {
                TechnicalSummary = "[Architect would design based on PM output]",
                ReadyForQA = true
            });
        }
    }

    internal sealed partial class QAEngineerMockExecutor() : Executor("QAEngineer")
    {
        [MessageHandler]
        private ValueTask<QAEngineerOutput> HandleAsync(
            ArchitectOutput archOutput, IWorkflowContext context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"🧪 QA Engineer: received {archOutput.TechnicalSummary} from Architect");

            return ValueTask.FromResult(new QAEngineerOutput
            {
                TestStrategy = "[QA would create test strategy]",
                ReadyForFeatureCard = true
            });
        }
    }

    internal sealed partial class FeatureCardMockExecutor() : Executor("FeatureCard")
    {
        [MessageHandler]
        private ValueTask<FeatureCard> HandleAsync(
            QAEngineerOutput qaOutput, IWorkflowContext context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("📦 Feature Card: aggregating all outputs");

            return ValueTask.FromResult(new FeatureCard
            {
                FeatureId = "FEAT-MOCK-0001",
                Title = "[Mock Feature Card]",
                Status = "Draft"
            });
        }
    }

}
