namespace Hyper.Controllers;

/// <summary>
/// Represents basic operations of a controller.
/// </summary>
internal interface IController
{
    /// <summary>
    /// Renders the objects controlled by the controller.
    /// </summary>
    void Render();

    /// <summary>
    /// Disposes of any resources or data used or controlled by the controller.
    /// </summary>
    void Dispose();
}