using Microsoft.SemanticKernel.Planning;

namespace PowerMatt.SKFromConfig.Extensions.Planner;

public interface IPlanner
{
    public Task<Plan> CreatePlanAsync(string goal);
}
