using Hud.Widgets.Colors;
using Hud.Widgets.NoChildren;
using Hud.Widgets.SingleChild;
using OpenTK.Mathematics;

namespace Hyper.Menu.Common;

/// <summary>
/// Button used in the game. It is the same as a <see cref="Button"/> but with some extra styling.
/// </summary>
internal class HyperButton : SingleChildWidget
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