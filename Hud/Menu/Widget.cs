using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hud.Menu;
public abstract class Widget
{
    public abstract Vector2 GetSize();

    public abstract void Render(Context context);
    
    public virtual void Click(Vector2 position) { }
    
    public virtual void KeyboardInput(Keys key) { }
}
