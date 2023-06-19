using Terminal.Gui;

namespace PowerMatt.Extensions.Gui.Views;

public class InputField
{
    private View _renderedView;
    private TextField _inputWindow;
    private Action<string> _onEnter;
    private View _parentView;

    public InputField(View parentView, Action<string> onEnter, Pos Y)
    {
        _parentView = parentView;

        // Create a frame view that will hold the text field
        _renderedView = new FrameView()
        {
            X = 0,
            Y = Y,
            Width = Dim.Fill(),
            Height = 3, // Increase the height to accommodate the border
            Border = new Border()
            {
                BorderStyle = BorderStyle.Single,
                BorderBrush = Color.White
            },
        };

        // Create the input window
        _inputWindow = new TextField("")
        {
            Width = Dim.Fill(),
            Height = 1
        };

        _inputWindow.KeyPress += (args) =>
        {
            if (args.KeyEvent.Key == Key.Enter && _inputWindow.Text.Length > 0)
            {
                AddMessage();
            }
        };

        _renderedView.Add(_inputWindow);
        _parentView.Add(_renderedView);

        _onEnter = onEnter;
    }

    public void Redraw()
    {
        //_parentView.Redraw(_renderedView.Bounds);
    }

    public void SetFocus()
    {
        if (_inputWindow != null)
        {
            //_inputWindow.SetFocus();
        }
    }

    private void AddMessage()
    {
        if (_inputWindow != null && _inputWindow.Text.Length > 0)
        {
            if (_onEnter != null)
            {
                _onEnter(_inputWindow.Text.ToString()!);
            }

            _inputWindow.Text = string.Empty; // Clear the input window
        }
    }
}