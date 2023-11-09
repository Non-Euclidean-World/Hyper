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

    private float _size;

    public string Content = "";

    private Vector4 _color;

    private int _caretPosition;

    private Stopwatch _stopwatch = new Stopwatch();

    private int _characterlimit;

    private readonly string _placeholder = "";

    public InputText(string text, float size, bool placeholder = false, int characterlimit = -1, Color color = Color.White)
    {
        _size = size;
        _characterlimit = characterlimit;
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
            Desactivate();
            return;
        }
        
        Activate();
    }

    public void Activate()
    {
        _isActive = true;
        _stopwatch.Start();
    }

    public void Desactivate()
    {
        _isActive = false;
        _stopwatch.Stop();
    }

    public override void KeyboardInput(KeyboardKeyEventArgs e)
    {
        if (!_isActive)
            return;

        var key = e.Key;
        
        if (key == Keys.Backspace)
        {
            if (_caretPosition > 0)
            {
                _caretPosition--;
                Content = Content.Remove(_caretPosition, 1);
            }
                
        }
        else if (key == Keys.Space)
        {
            AddCharacter(" ");
        }
        else if (key is >= Keys.A and <= Keys.Z)
        {
            if (e.Shift)
                AddCharacter(key.ToString());
            else
                AddCharacter(key.ToString().ToLower());
        }
        else if (key is >= Keys.D0 and <= Keys.D9)
        {
            AddCharacter(key.ToString().Substring(1));
        }
        else if (key is Keys.Left)
        {
            _caretPosition = Math.Max(0, _caretPosition - 1);
            _stopwatch.Restart();
        }
        else if (key is Keys.Right)
        {
            _caretPosition = Math.Min(Content.Length, _caretPosition + 1);
            _stopwatch.Restart();
        }
    }

    private void AddCharacter(string c)
    {
        if (Content.Length == _characterlimit) 
            return;
        Content = Content.Insert(_caretPosition, c);
        _caretPosition++;
    }
}