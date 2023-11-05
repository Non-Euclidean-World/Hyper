﻿using OpenTK.Mathematics;

namespace Hud.Menu.SingleChild;

public class ClickDetector : SingleChildWidget
{
    private readonly Action _action;

    private Vector2 _position;
    
    private Vector2 _size;

    public ClickDetector(Widget child, Action action) : base(child)
    {
        _action = action;
    }

    public override Vector2 GetSize()
    {
        _size = Child.GetSize();
        return _size;
    }

    public override void Render(Context context)
    {
        _position = context.Position;
        Child.Render(context);
    }
    
    public override void Click(Vector2 position)
    {
        if (_position.X + _size.X < position.X ||
            _position.X > position.X ||
            _position.Y + _size.Y < position.Y ||
            _position.Y > position.Y)
        {
            return;
        }
        
        _action.Invoke();
    }
}