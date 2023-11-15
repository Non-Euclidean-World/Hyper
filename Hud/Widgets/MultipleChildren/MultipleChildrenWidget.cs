using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Hud.Widgets.MultipleChildren;

/// <summary>
/// Class that widgets with multiple children inherit from.
/// </summary>
public abstract class MultipleChildrenWidget : Widget
{
    protected readonly Widget[] Children;

    protected MultipleChildrenWidget(Widget[] children) { Children = children; }

    public override void Click(Vector2 position)
    {
        foreach (var child in Children)
        {
            child.Click(position);
        }
    }

    public override void KeyboardInput(KeyboardKeyEventArgs e)
    {
        foreach (var child in Children)
        {
            child.KeyboardInput(e);
        }
    }
}