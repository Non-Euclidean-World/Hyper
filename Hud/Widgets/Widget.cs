using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Hud.Widgets;
/// <summary>
/// Base class for all widgets. Can be used to render 2d elements on the screen.
/// </summary>
public abstract class Widget
{
    /// <summary>
    /// Gets the minimum size the widget can be.
    /// </summary>
    /// <returns></returns>
    public abstract Vector2 GetSize();

    /// <summary>
    /// Renders the widget.
    /// </summary>
    /// <param name="context">Context holds all the information needed to render the widget.</param>
    public abstract void Render(Context context);

    /// <summary>
    /// Method that is called when the user click on the screen. It is supposed to be propagated to the children.
    /// </summary>
    /// <param name="position">The position on the screen the user clicked.</param>
    public virtual void Click(Vector2 position) { }

    /// <summary>
    /// Method that is called when the user presses any key. It is supposed to be propagated to the children.
    /// </summary>
    /// <param name="e">Describes the keypress.</param>
    public virtual void KeyboardInput(KeyboardKeyEventArgs e) { }
}
