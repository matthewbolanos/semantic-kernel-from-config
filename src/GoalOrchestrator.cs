using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using static PowerMatt.SKFromConfig.Extensions.Agent.GoalThread;

namespace PowerMatt.SKFromConfig.Extensions.Agent;


public class GoalOrchestrator
{
    private ILogger<ConsoleAgent>? logger;

    public GoalOrchestrator(ILogger<ConsoleAgent>? logger = null)
    {
        this.logger = logger;
    }

    public async IAsyncEnumerable<OrchestratorMessage> ReceiveMessage(IKernel kernel, SKContext context)
    {
        SKContext c = new SKContext();
        c["history"] = context["history"];

        // Message is always useful for now
        yield return new OrchestratorMessage(OrchestratorMessageType.USEFUL_MESSAGE);

        // Get the goal
        var goal = (
            await kernel.Skills.GetFunction("LifeCoach", "GetUsersGoal")
            .InvokeAsync(c)
        ).ToString().Trim();
        c["goal"] = goal;
        logger?.LogInformation(c["goal"] + $": Created()");
        yield return new OrchestratorMessage(OrchestratorMessageType.UPDATE_GOAL, goal);

        // If there is a goal, create a plan to achieve the goal
        // if (!String.IsNullOrEmpty(goal) && !goal.Contains("NOT KNOWN"))
        // {
        //     SequentialPlanner planner = new SequentialPlanner(kernel);
        //     var plan = await planner.CreatePlanAsync(goal);
        //     logger?.LogInformation(c["goal"] + $": Plan({plan.ToJson(true)})");

        //     var response = (await plan.InvokeAsync(c)).ToString().Trim();
        //     logger?.LogInformation(c["goal"] + $": Response({response})");
        //     yield return new OrchestratorMessage(
        //         OrchestratorMessageType.REPLY_TO_USER,
        //         response);
        // }
        // else
        // {

        // Chat with user
        var response = (
            await kernel.Skills.GetFunction("Communicator", "ChatWithUser")
            .InvokeAsync(c)
        ).ToString().Trim();
        logger?.LogInformation(c["goal"] + $": Response({response})");
        yield return new OrchestratorMessage(
            OrchestratorMessageType.REPLY_TO_USER,
            response);

        // }
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
