using Terminal.Gui;

namespace PowerMatt.Gui.Views;

public class ChatView : Window
{
    private Action<string> _onInput;
    private MessageHistory _messageHistory;

    public ChatView(Action<string> onInput)
    {
        Title = "Semantic Kernel (Ctrl+Q to quit)";

        ColorScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.White, Color.Black)
        };

        _onInput += onInput;

        _messageHistory = new MessageHistory(this);

        Action<string> onEnter = (string input) =>
        {
            // Create a new message
            _messageHistory.AddMessage(input, "You", DateTime.Now);
            onInput.Invoke(input);
        };

        // Create the input window
        var InputField = new InputField(this, onEnter, _messageHistory.Bottom);

        InputField.SetFocus();
    }

    public void Respond(string reply)
    {
        // Create a new message
        _messageHistory.AddMessage(reply, "Bot", DateTime.Now);
    }
}