using Microsoft.SemanticKernel.Orchestration;
using static PowerMatt.SKFromConfig.Extensions.Agent.GoalThread;

namespace PowerMatt.SKFromConfig.Extensions.Agent;

public class GoalOrchestrator
{
    public async IAsyncEnumerable<OrchestratorMessage> ReceiveMessage(SKContext context)
    {
        yield return new OrchestratorMessage(OrchestratorMessageType.USEFUL_MESSAGE);


        try { context["index"] = context["index"]; }
        catch { context["index"] = "0"; }

        context["index"] = (int.Parse(context["index"]) + 1).ToString();

        await Task.Delay(100);

        yield return new OrchestratorMessage(
            OrchestratorMessageType.REPLY_TO_USER,
            "The main function has been called in the same thread " + context["index"] + " times.");

        if (context["index"] == "3")
        {
            yield return new OrchestratorMessage(OrchestratorMessageType.GOAL_ACHIEVED);
        }
        if (context["input"] == "cancel")
        {
            yield return new OrchestratorMessage(OrchestratorMessageType.GOAL_CANCELED);
        }
    }

    public class OrchestratorMessage
    {
        public OrchestratorMessageType Type { get; set; }
        public string? Message { get; set; }

        public OrchestratorMessage(OrchestratorMessageType type, string? message = null)
        {
            Type = type;
            Message = message;
        }
    }
}
