using OpenTK.Mathematics;

namespace Hud.Widgets.SingleChild;
/// <summary>
/// A widget that has a fixed size.
/// </summary>
public class SizeBox : SingleChildWidget
{
    private readonly Vector2 _size;

    /// <summary>
    /// Creates an instance of SizeBox class.
    /// </summary>
    /// <param name="child">The widget inside SizeBox.</param>
    /// <param name="size">The size of the SizeBox.</param>
    public SizeBox(Widget child, Vector2 size) : base(child)
    {
        _size = size;
    }

    public override Vector2 GetSize()
    {
        return _size;
    }

    public override void Render(Context context)
    {
        Child.Render(new Context(context, context.Position, _size));
    }
}