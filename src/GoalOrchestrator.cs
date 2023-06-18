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
        c["input"] = context["input"];
        c["history"] = context["history"];
        c["goal"] = context["goal"];

        // Check if there is a goal
        if (String.IsNullOrEmpty(c["goal"]) || c["goal"] == "GOAL IS NOT KNOWN")
        {
            yield return new OrchestratorMessage(OrchestratorMessageType.USEFUL_MESSAGE);

            c["input"] = context["input"];
            c["history"] = context["history"];
            var goal = (
                await kernel.Skills.GetFunction("LifeCoach", "GetUsersGoal")
                .InvokeAsync(c)
            ).ToString().Trim();

            logger?.LogInformation($"Goal: '{goal}'");

            yield return new OrchestratorMessage(OrchestratorMessageType.UPDATE_GOAL, goal);
        }
        else
        {
            // Check if message helps with goal
            c["input"] = context["input"];
            var DoesMessageHelpWithGoal = (
                await kernel.Skills.GetFunction("LifeCoach", "CheckIfMessageHelpsWithGoal")
                .InvokeAsync(c)
            ).ToString().Trim();

            logger?.LogInformation($"DoesMessageHelpWithGoal: '{DoesMessageHelpWithGoal}'");

            if (bool.Parse(DoesMessageHelpWithGoal))
            {
                yield return new OrchestratorMessage(OrchestratorMessageType.USEFUL_MESSAGE);
            }
            else
            {
                yield return new OrchestratorMessage(OrchestratorMessageType.NOT_USEFUL_MESSAGE);
                yield break;
            }
        }

        // Chat with user
        var response = (
            await kernel.Skills.GetFunction("Communicator", "ChatWithUser")
            .InvokeAsync(c)
        ).ToString().Trim();

        yield return new OrchestratorMessage(
            OrchestratorMessageType.REPLY_TO_USER,
            response);

        // Check if goal was achieved
        var WasGoalAchieved = (
            await kernel.Skills.GetFunction("LifeCoach", "CheckIfGoalWasAchieved")
            .InvokeAsync(c)
        ).ToString().Trim();

        logger?.LogInformation($"WasGoalAchieved: '{WasGoalAchieved}'");

        if (bool.Parse(WasGoalAchieved))
        {
            yield return new OrchestratorMessage(OrchestratorMessageType.GOAL_ACHIEVED);
        }

        // Check if goal was not able to be completed
        var DoesUserWantToCancelGoal = (
            await kernel.Skills.GetFunction("LifeCoach", "CheckIfUserWantsToGiveUp")
            .InvokeAsync(c)
        ).ToString().Trim();

        logger?.LogInformation($"DoesUserWantToCancelGoal: '{DoesUserWantToCancelGoal}'");

        if (bool.Parse(DoesUserWantToCancelGoal))
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
