using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using PowerMatt.SKFromConfig.Extensions.Agent;

namespace PowerMatt.Sample;


public class PlannerOrchestrator : IAgentOrchestrator
{
    private ILogger<ConsoleAgent>? logger;

    public PlannerOrchestrator(ILogger<ConsoleAgent>? logger = null)
    {
        this.logger = logger;
    }

    public async IAsyncEnumerable<OrchestratorMessage> ReceiveMessage(IKernel kernel, SKContext context)
    {
        SKContext c = new SKContext();
        c["history"] = context["history"];

        // TODO: Add planner

        var response = (
            await kernel.Skills.GetFunction("Communicator", "ChatWithUser")
            .InvokeAsync(c)
        ).ToString().Trim();
        logger?.LogInformation(c["goal"] + $": Response({response})");

        yield return new OrchestratorMessage(
            OrchestratorMessageType.REPLY_TO_USER,
            response);
    }
}
