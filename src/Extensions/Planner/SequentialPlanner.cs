using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;

namespace PowerMatt.SKFromConfig.Extensions.Planner;

public class SequentialPlanner : IPlanner
{
    private Microsoft.SemanticKernel.Planning.SequentialPlanner sequentialPlanner;

    public SequentialPlanner(IKernel kernel)
    {
        sequentialPlanner = new Microsoft.SemanticKernel.Planning.SequentialPlanner(kernel);
    }

    public async Task<Plan> CreatePlanAsync(string goal)
    {
        return await sequentialPlanner.CreatePlanAsync(goal);
    }
}
