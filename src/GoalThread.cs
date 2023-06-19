using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using static PowerMatt.SKFromConfig.Extensions.Agent.GoalOrchestrator;

namespace PowerMatt.SKFromConfig.Extensions.Agent;

public class GoalThread
{
    private IKernel _kernel;

    public SKContext Context { get; set; }
    private GoalOrchestrator _orchestrator;

    private string _goal = "";

    public GoalThread(
        IKernel kernel,
        SKContext initialContext,
        GoalOrchestrator orchestrator)
    {
        _kernel = kernel;
        Context = initialContext;
        _orchestrator = orchestrator;
    }

    public void StartAsync()
    {

    }

    public async IAsyncEnumerable<OrchestratorMessage> ReceiveMessage(string message)
    {
        // prepare the context
        Context["input"] = message;
        Context["goal"] = _goal;

        // run the orchestrator on the user's message
        await foreach (var orchestratorMessage in _orchestrator.ReceiveMessage(_kernel, Context))
        {
            switch (orchestratorMessage.Type)
            {
                case OrchestratorMessageType.UPDATE_GOAL:
                    _goal = orchestratorMessage.Message!;
                    break;
            }
            yield return orchestratorMessage;
        }
    }

    public enum OrchestratorMessageType
    {
        USEFUL_MESSAGE,
        NOT_USEFUL_MESSAGE,
        REPLY_TO_USER,
        UPDATE_GOAL,
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
