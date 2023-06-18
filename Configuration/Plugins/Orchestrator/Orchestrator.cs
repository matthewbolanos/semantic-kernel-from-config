using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace PowerMatt.Sample.Plugin;

public class Orchestrator
{
    [SKFunction("Main function; DO NOT USE FOR PLANS")]
    public string Main(SKContext context)
    {
        // Check if index exists
        try { context["index"] = context["index"]; }
        catch { context["index"] = "0"; }

        context["index"] = (int.Parse(context["index"]) + 1).ToString();

        if (context["index"] == "3")
        {
            context["GoalAchieved"] = "TRUE";
        }

        return @"The main function has been called in the same thread " + context["index"] + " times.";
    }

    [SKFunction("Will wait for the response from the user and return it as a string. ")]
    public string GetUserResponse()
    {
        return @"I'm sleepy";
    }

    [SKFunction("Will update and revise the current plan after getting user input.")]
    public string UpdatePlan(string input)
    {
        return @"I'm a new plan";
    }

    [SKFunction("Will create a brand new plan from scratch based on the user's input.")]
    public string CreatePlan(string input)
    {
        return @"I'm a new plan";
    }
}