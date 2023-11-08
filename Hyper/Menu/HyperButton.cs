using Hud.Widgets.Colors;
using Hud.Widgets.NoChildren;
using Hud.Widgets.SingleChild;
using OpenTK.Mathematics;

namespace Hyper.Menu;

public class HyperButton : SingleChildWidget
{
    public HyperButton(string text, Action action, Vector2 size)
    {
        Child = new Padding(
            size: 0.01f,
            child: new Background(
                color: Color.Secondary,
                child: new Padding(
                    size: 0.01f,
                    child: new Button(
                        size: size - new Vector2(0.02f),
                        action: () => action?.Invoke(),
                        text: text,
                        color: Color.Primary
                    )
                )
            )
        );
    }
}