using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using static PowerMatt.SKFromConfig.Extensions.Agent.GoalOrchestrator;

namespace PowerMatt.SKFromConfig.Extensions.Agent;

public class GoalThread
{
    private Action<string> _respondToUser;
    private IKernel _kernel;
    private SKContext _context;
    private GoalOrchestrator _orchestrator;

    public GoalThread(
        IKernel kernel,
        SKContext initialContext,
        GoalOrchestrator orchestrator,
        Action<string> respondToUser)
    {
        _kernel = kernel;
        _respondToUser = respondToUser;
        _context = initialContext;
        _orchestrator = orchestrator;
    }

    public void StartAsync()
    {

    }

    public async IAsyncEnumerable<OrchestratorMessage> ReceiveMessage(string message)
    {
        // prepare the context
        _context["input"] = message;

        // run the orchestrator on the user's message
        await foreach (var orchestratorMessage in _orchestrator.ReceiveMessage(_context))
        {
            yield return orchestratorMessage;
        }
    }

    public enum OrchestratorMessageType
    {
        USEFUL_MESSAGE,
        NOT_USEFUL_MESSAGE,
        REPLY_TO_USER,
        CREATE_NEW_GOAL,
        GOAL_ACHIEVED,
        GOAL_CANCELED,
        GOAL_NOT_ABLE_TO_COMPLETED,
        ERROR,
    }
    public enum GoalEndState
    {
        SUCCESS,
        CANCEL,
        NOT_ABLE_TO_COMPLETE,
        ERROR
    }

}
