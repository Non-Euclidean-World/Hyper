using OpenTK.Mathematics;

namespace Hud.Widgets.SingleChild;
/// <summary>
/// A widget that performs an action when clicked.
/// </summary>
public class ClickDetector : SingleChildWidget
{
    private readonly Action _action;

    private readonly Action _outsideAction;

    private Vector2 _position;

    private Vector2 _size;

    private readonly bool _propagate;

    /// <summary>
    /// Creates an instance of ClickDetector.
    /// </summary>
    /// <param name="child">Widget inside ClickDetector.</param>
    /// <param name="action">An action that is performed after ClickDetector is pressed.</param>
    /// <param name="outsideAction">An action that is performed when area outside ClickDetector is pressed.</param>
    /// <param name="propagate">Whether after being clicked .Click() should be called on the child.</param>
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
            _outsideAction.Invoke();
            if (_propagate)
                Child.Click(position);
            return;
        }

        if (_propagate)
            Child.Click(position);
        _action.Invoke();
    }
}