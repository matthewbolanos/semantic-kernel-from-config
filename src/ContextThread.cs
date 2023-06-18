using Microsoft.SemanticKernel.Orchestration;

namespace PowerMatt.SKFromConfig.Extensions.Agent;

public class ContextThread
{
    private Action<string> _respondToUser;
    private SKContext _context;

    private TaskCompletionSource<string>? _userInputCompletionSource;

    public ContextThread(SKContext initialContext, Action<string> respondToUser)
    {
        _respondToUser = respondToUser;
        _context = initialContext;
    }

    public async Task StartAsync()
    {
        _respondToUser(_context["input"]);

        // wait for user input
        _userInputCompletionSource = new TaskCompletionSource<string>();

        string userInput = await _userInputCompletionSource.Task;
        _respondToUser("I received your input: " + userInput);
    }

    public bool IsWaitingForUserInput()
    {
        return _userInputCompletionSource != null;
    }

    public void ReceiveMessage(string message)
    {
        if (_userInputCompletionSource == null)
        {
            throw new InvalidOperationException("Cannot receive message when not waiting for user input.");
        }

        _userInputCompletionSource.SetResult(message);
        _userInputCompletionSource = null;
    }

}
