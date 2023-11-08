using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hud.Widgets.SingleChild;

public abstract class SingleChildWidget : Widget
{
    protected Widget Child;
    
    protected SingleChildWidget(Widget child) { Child = child; }
    
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
    
    public override void KeyboardInput(Keys key)
    {
        Child.KeyboardInput(key);
    }
}