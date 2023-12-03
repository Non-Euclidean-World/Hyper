using Hud.Widgets.Colors;
using Hud.Widgets.NoChildren;
using Hud.Widgets.SingleChild;
using OpenTK.Mathematics;

namespace Hyper.Menu.Common;

public class HyperText : SingleChildWidget
{
    public HyperText(string text, Vector2 size)
    {
        Child = new Padding(
            size: 0.005f,
            child: new Background(
                color: Color.Secondary,
                child: new Padding(
                    size: 0.005f,
                    child: new SizeBox(
                        size: size - new Vector2(0.01f),
                        child: new Background(
                            color: Color.Primary,
                            child: new Center(
                                child: new Text(
                                    size: (size.Y - 0.01f) * 1 / 3,
                                    text: text
                                )
                            )
                        )
                    )
                )
            )
        );
    }
}