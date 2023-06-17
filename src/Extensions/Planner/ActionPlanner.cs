using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;

namespace PowerMatt.SKFromConfig.Extensions.Planner;

public class ActionPlanner : IPlanner
{
    private Microsoft.SemanticKernel.Planning.ActionPlanner planner;

    public ActionPlanner(IKernel kernel)
    {
        planner = new Microsoft.SemanticKernel.Planning.ActionPlanner(kernel);
    }

    public async Task<Plan> CreatePlanAsync(string goal)
    {
        return await planner.CreatePlanAsync(goal);
    }
}
