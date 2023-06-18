using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace PowerMatt.SKFromConfig.Extensions.Agent;

public class ContextThread
{
    private Action<string> _respondToUser;
    private IKernel _kernel;
    private SKContext _context;
    private ISKFunction _mainFunction;

    private TaskCompletionSource<string>? _userInputCompletionSource;

    public ContextThread(
        IKernel kernel,
        SKContext initialContext,
        ISKFunction mainFunction,
        Action<string> respondToUser)
    {
        _kernel = kernel;
        _respondToUser = respondToUser;
        _context = initialContext;
        _mainFunction = mainFunction;
    }

    public async Task StartAsync()
    {
        // Loop until the goal is achieved
        while (!bool.Parse(_context["GoalAchieved"]))
        {
            // run main function
            var result = await _mainFunction.InvokeAsync(_context);
            _respondToUser(result.ToString());

            if (!bool.Parse(_context["GoalAchieved"]))
            {
                // wait for user input
                _userInputCompletionSource = new TaskCompletionSource<string>();
                _context["input"] = await _userInputCompletionSource.Task;
            }
        }
    }

    public bool IsWaitingForUserInput()
    {
        var isWaiting = _userInputCompletionSource != null && !_userInputCompletionSource.Task.IsCompleted;
        return isWaiting;
    }

    public void ReceiveMessage(string message)
    {
        if (!IsWaitingForUserInput())
        {
            throw new InvalidOperationException("Cannot receive message when not waiting for user input.");
        }

        _userInputCompletionSource!.SetResult(message);
    }

}
