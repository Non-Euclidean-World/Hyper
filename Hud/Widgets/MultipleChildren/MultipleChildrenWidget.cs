using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Hud.Widgets.MultipleChildren;

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

    public override void KeyboardInput(KeyboardKeyEventArgs key)
    {
        foreach (var child in Children)
        {
            child.KeyboardInput(key);
        }
    }
}