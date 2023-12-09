using System.Diagnostics;
using Hud.Widgets.Colors;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
namespace Hud.Widgets.NoChildren;

// This class is only used by InputTextBox and should not be used outside of it.
internal class InputText : Widget
{
    private bool _isActive = false;

    private Vector2 _position;

    private Vector2 _boxSize;

    private readonly float _size;

    /// <summary>
    /// The content of the text.
    /// </summary>
    public string Content = "";

    private readonly Vector4 _color;

    private int _caretPosition;

    private readonly Stopwatch _stopwatch = new Stopwatch();

    private readonly int _characterLimit;

    private readonly string _placeholder = "";

    /// <summary>
    /// Creates an instance of InputText class.
    /// </summary>
    /// <param name="text">The initial text.</param>
    /// <param name="size">The size of the widget.</param>
    /// <param name="placeholder">Whether the initial text is a placeholder.</param>
    /// <param name="characterLimit">The maximum number of characters. If negative there is no limit.</param>
    /// <param name="color">The color of the widget.</param>
    public InputText(string text, float size, bool placeholder = false, int characterLimit = -1, Color color = Color.White)
    {
        _size = size;
        _characterLimit = characterLimit;
        _color = ColorGetter.GetVector(color);
        if (placeholder)
        {
            _placeholder = text;
            _caretPosition = 0;
        }
        else
        {
            Content = text;
            _caretPosition = text.Length;
        }
    }

    public override Vector2 GetSize()
    {
        if (Content.Length == 0 && !_isActive)
            return Printer.GetTextSize(_placeholder, _size);

        return Printer.GetTextSize(Content, _size);
    }

    public override void Render(Context context)
    {
        _position = context.Position;
        _boxSize = context.Size;

        context.Shader.UseTexture(false);
        context.Shader.SetColor(_color);

        string text = Content;
        if (text.Length == 0 && !_isActive)
            text = _placeholder;
        if (_isActive && _stopwatch.ElapsedMilliseconds % 1000 < 500)
        {
            text = text.Insert(_caretPosition, Printer.Caret.ToString());
        }
        Printer.RenderStringTopLeft(context.Shader, text, _size, context.Position.X, context.Position.Y);
    }

    public override void Click(Vector2 position)
    {
        if (_position.X + _boxSize.X < position.X ||
            _position.X > position.X ||
            _position.Y - _boxSize.Y > position.Y ||
            _position.Y < position.Y)
        {
            Deactivate();
            return;
        }

        Activate();
    }

    public override void KeyboardInput(KeyboardKeyEventArgs e)
    {
        if (!_isActive)
            return;

        var key = e.Key;

        switch (key)
        {
            case Keys.Backspace:
                {
                    if (_caretPosition > 0)
                    {
                        _caretPosition--;
                        Content = Content.Remove(_caretPosition, 1);
                    }
                    break;
                }
            case Keys.Space:
                AddCharacter(" ");
                break;
            case >= Keys.A and <= Keys.Z when e.Shift:
                AddCharacter(key.ToString());
                break;
            case >= Keys.A and <= Keys.Z:
                AddCharacter(key.ToString().ToLower());
                break;
            case >= Keys.D0 and <= Keys.D9:
                AddCharacter(key.ToString().Substring(1));
                break;
            case Keys.Left:
                _caretPosition = Math.Max(0, _caretPosition - 1);
                _stopwatch.Restart();
                break;
            case Keys.Right:
                _caretPosition = Math.Min(Content.Length, _caretPosition + 1);
                _stopwatch.Restart();
                break;
        }
    }

    public void Activate()
    {
        _isActive = true;
        _stopwatch.Start();
    }

    public void Deactivate()
    {
        _isActive = false;
        _stopwatch.Stop();
    }

    private void AddCharacter(string c)
    {
        if (Content.Length == _characterLimit)
            return;
        Content = Content.Insert(_caretPosition, c);
        _caretPosition++;
    }
}