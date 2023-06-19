using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

namespace PowerMatt.SKFromConfig.Extensions.Agent;


public interface IAgentOrchestrator
{
    public IAsyncEnumerable<OrchestratorMessage> ReceiveMessage(IKernel kernel, SKContext context);
}
