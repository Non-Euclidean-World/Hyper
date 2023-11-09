using System.Diagnostics;
using Hud.Widgets.Colors;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
namespace Hud.Widgets.NoChildren;

public class InputTextBox : Widget
{
    private bool _selected = false;

    private Vector2 _position;
    
    private Vector2 _boxSize;

    private float _size;

    public string Text;

    public Vector4 Color;

    private int _caretPosition;

    private Stopwatch _stopwatch = new Stopwatch();

    public InputTextBox(string text, float size, Color color = Colors.Color.White)
    {
        _caretPosition = text.Length;
        Text = text;
        _size = size;
        Color = ColorGetter.GetVector(color);
    }

    public override Vector2 GetSize()
    {
        return Printer.GetTextSize(Text, _size);
    }

    public override void Render(Context context)
    {
        _position = context.Position;
        _boxSize = context.Size;

        context.Shader.UseTexture(false);
        context.Shader.SetColor(Color);

        string text = Text;
        if (_selected && _stopwatch.ElapsedMilliseconds % 1000 < 500)
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
            _selected = false;
            _stopwatch.Stop();
            return;
        }
        
        _selected = true;
        _stopwatch.Start();
    }
    
    public override void KeyboardInput(Keys key)
    {
        if (!_selected)
            return;
        
        if (key == Keys.Backspace)
        {
            if (Text.Length > 0)
            {
                _caretPosition--;
                Text = Text.Remove(_caretPosition, 1);
            }
                
        }
        else if (key == Keys.Space)
        {
            Text = Text.Insert(_caretPosition, " ");
            _caretPosition++;
        }
        else if (key is >= Keys.A and <= Keys.Z)
        {
            Text = Text.Insert(_caretPosition, key.ToString());
            _caretPosition++;
        }
        else if (key is >= Keys.D0 and <= Keys.D9)
        {
            Text = Text.Insert(_caretPosition, key.ToString().Substring(1));
            _caretPosition++;
        }
        else if (key is Keys.Left)
        {
            _caretPosition = Math.Max(0, _caretPosition - 1);
            _stopwatch.Restart();
        }
        else if (key is Keys.Right)
        {
            _caretPosition = Math.Min(Text.Length, _caretPosition + 1);
            _stopwatch.Restart();
        }
    }
}