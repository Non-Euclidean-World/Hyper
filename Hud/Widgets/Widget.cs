using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Hud.Widgets;
public abstract class Widget
{
    public abstract Vector2 GetSize();

    public abstract void Render(Context context);

    public virtual void Click(Vector2 position) { }

    public virtual void KeyboardInput(KeyboardKeyEventArgs key) { }
}
