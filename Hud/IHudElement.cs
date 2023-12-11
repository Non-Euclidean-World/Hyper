using Common;

namespace Hud;
/// <summary>
/// Provides a way to render a 2D element on the screen.
/// </summary>
public interface IHudElement : IDisposable
{
    /// <summary>
    /// Whether or not the element is visible.
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// Renders the element.
    /// </summary>
    /// <param name="shader">The shader used for rendering.</param>
    public void Render(Shader shader);
}
