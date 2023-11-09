using Hud.Widgets.Colors;
using Hud.Widgets.SingleChild;
using OpenTK.Mathematics;

namespace Hud.Widgets.NoChildren;
/// <summary>
/// A button that does something when clicked.
/// </summary>
public class Button : SingleChildWidget
{
    /// <summary>
    /// Creates an instance of Button class.
    /// </summary>
    /// <param name="size">The size of the button.</param>
    /// <param name="action">The action that is performed when the button is pressed.</param>
    /// <param name="text">Text on the button.</param>
    /// <param name="color">Color of the button background.</param>
    public Button(Vector2 size, Action action, string text, Color color = Color.Primary)
    {
        Child = new SizeBox(
            size: size,
            child: new ClickDetector(
                action: action,
                child: new Background(
                    color: color,
                    child: new Center(
                        child: new Text(
                            size: size.Y * 1 / 3,
                            text: text
                        )
                    )
                )
            )
        );
    }
}
