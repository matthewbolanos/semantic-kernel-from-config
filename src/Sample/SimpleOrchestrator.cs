using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using PowerMatt.SKFromConfig.Extensions.Agent;

namespace PowerMatt.Sample;


public class SimpleOrchestrator : IAgentOrchestrator
{
    private ILogger<ConsoleAgent>? logger;

    public SimpleOrchestrator(ILogger<ConsoleAgent>? logger = null)
    {
        this.logger = logger;
    }

    public async IAsyncEnumerable<OrchestratorMessage> ReceiveMessage(IKernel kernel, SKContext context)
    {
        SKContext c = new SKContext();
        c["history"] = context["history"];

        var response = (
            await kernel.Skills.GetFunction("Communicator", "ChatWithUser")
            .InvokeAsync(c)
        ).ToString().Trim();
        logger?.LogInformation($": Response({response})");

        yield return new OrchestratorMessage(
            OrchestratorMessageType.REPLY_TO_USER,
            response);
    }
}
