using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;

namespace PowerMatt.SKFromConfig.Extensions.Planner;

public class MockPlanner : IPlanner
{
    private IKernel _kernel;

    public MockPlanner(IKernel kernel)
    {
        _kernel = kernel;
    }

    public Task<Plan> CreatePlanAsync(string goal)
    {
        return Task.FromResult<Plan>(
            new Plan(goal, _kernel.Skills.GetFunction("Communicator", "CasuallyRespondToUser")!)
        );
    }
}
