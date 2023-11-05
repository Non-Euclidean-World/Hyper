using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hud.Menu;

public abstract class MultipleChildWidget : Widget
{
    protected readonly Widget[] Children;
    
    protected MultipleChildWidget(Widget[] children) { Children = children; }

    public override void Click(Vector2 position)
    {
        foreach (var child in Children)
        {
            child.Click(position);
        }
    }
    
    public override void KeyboardInput(Keys key)
    {
        foreach (var child in Children)
        {
            child.KeyboardInput(key);
        }
    }
}