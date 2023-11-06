using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hud.Widgets.Colors;
using Hud.Widgets.SingleChild;
using OpenTK.Mathematics;

namespace Hud.Widgets.NoChildren;
public class Button : Widget
{
    private Widget _child;

    public Button(Vector2 size, Action action, string text, Color color = Color.Blue) 
    {
        _child = new SizeBox(
                size: size,
                child: new ClickDetector(
                    action: action,
                    child: new Background(
                        color: color,
                        child: new Center(
                                child: new TextBox(
                                    size: size.Y * 2/3,
                                    text: text
                                )
                            )
                        )
                    )
                );
    }

    public override Vector2 GetSize()
    {
        return _child.GetSize();
    }

    public override void Render(Context context)
    {
        _child.Render(context);
    }
}
