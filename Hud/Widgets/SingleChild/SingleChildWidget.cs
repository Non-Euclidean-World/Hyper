using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Hud.Widgets.SingleChild;
/// <summary>
/// Class that widgets with a single child inherit from.
/// </summary>
public abstract class SingleChildWidget : Widget
{
    protected Widget Child;

    /// <summary>
    /// Creates a widget with a child.
    /// </summary>
    /// <param name="child">The child.</param>
    protected SingleChildWidget(Widget child) { Child = child; }

    /// <summary>
    /// Creates a widget without initializing the child. This is used for widgets that create their child in the constructor.
    /// </summary>
    protected SingleChildWidget() { }

    public override void Render(Context context)
    {
        Child.Render(context);
    }

    public override Vector2 GetSize()
    {
        return Child.GetSize();
    }

    public override void Click(Vector2 position)
    {
        Child.Click(position);
    }

    public override void KeyboardInput(KeyboardKeyEventArgs e)
    {
        Child.KeyboardInput(e);
    }
}