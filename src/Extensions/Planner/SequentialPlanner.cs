using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;

namespace PowerMatt.SKFromConfig.Extensions.Planner;

public class SequentialPlanner : IPlanner
{
    private Microsoft.SemanticKernel.Planning.SequentialPlanner planner;

    public SequentialPlanner(IKernel kernel)
    {
        planner = new Microsoft.SemanticKernel.Planning.SequentialPlanner(kernel);
    }

    public async Task<Plan> CreatePlanAsync(string goal)
    {
        return await planner.CreatePlanAsync(goal);
    }
}
