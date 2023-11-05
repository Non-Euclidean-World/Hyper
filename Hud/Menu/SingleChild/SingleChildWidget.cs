using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hud.Menu.SingleChild;

public abstract class SingleChildWidget : Widget
{
    protected readonly Widget Child;
    
    protected SingleChildWidget(Widget child) { Child = child; }
    
    public override void Click(Vector2 position)
    {
        Child.Click(position);
    }
    
    public override void KeyboardInput(Keys key)
    {
        Child.KeyboardInput(key);
    }
}