using OpenTK.Mathematics;

namespace Hud.Widgets.SingleChild;

public class ClickDetector : SingleChildWidget
{
    private readonly Action _action;

    private readonly Action _outsideAction;

    private Vector2 _position;
    
    private Vector2 _size;

    private bool _propagate;

    public ClickDetector(Widget child, Action action, Action outsideAction = null!, bool propagate = false) : base(child)
    {
        _action = action;
        _outsideAction = outsideAction;
        _propagate = propagate;
    }

    public override Vector2 GetSize()
    {
        return _size;
    }

    public override void Render(Context context)
    {
        _size = context.Size;
        _position = context.Position;
        Child.Render(context);
    }
    
    public override void Click(Vector2 position)
    {
        if (_position.X + _size.X < position.X ||
            _position.X > position.X ||
            _position.Y - _size.Y > position.Y ||
            _position.Y < position.Y)
        {
            _outsideAction?.Invoke();
            if (_propagate)
                Child.Click(position);
            return;
        }

        if (_propagate)
            Child.Click(position);
        _action.Invoke();
    }
}