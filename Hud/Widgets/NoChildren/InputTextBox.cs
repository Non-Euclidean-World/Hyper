using Hud.Widgets.Colors;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hud.Widgets.NoChildren;

public class InputTextBox : TextBox
{
    private bool _selected = false;

    private Vector2 _position;
    
    private Vector2 _size;
    
    public InputTextBox(string text, float size, Color color = Color.White) : base(text, size, color)
    {
    }
    
    public override Vector2 GetSize()
    {
        _size = base.GetSize();
        return _size;
    }

    public override void Render(Context context)
    {
        _position = context.Position;
        base.Render(context);
    }

    public override void Click(Vector2 position)
    {
        if (_position.X + _size.X < position.X ||
            _position.X > position.X ||
            _position.Y + _size.Y < position.Y ||
            _position.Y > position.Y)
        {
            _selected = false;
            return;
        }
        
        _selected = true;
    }
    
    public override void KeyboardInput(Keys key)
    {
        if (!_selected)
            return;
        
        if (key == Keys.Backspace)
        {
            if (Text.Length > 0)
                Text = Text[..^1];
        }
        else if (key == Keys.Space)
        {
            Text += " ";
        }
        else if (key is >= Keys.A and <= Keys.Z)
        {
            Text += key.ToString().ToLower();
        }
    }
}