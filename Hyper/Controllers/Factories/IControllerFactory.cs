using Common;

namespace Hyper.Controllers.Factories;

/// <summary>
/// Represents a factory interface responsible for creating controllers.
/// </summary>
internal interface IControllerFactory
{
    /// <summary>
    /// Creates an array of controllers based on the provided settings.
    /// </summary>
    /// <param name="settings">The settings used to configure the controllers<./param>
    /// <returns>An array of controllers instantiated based on the settings.</returns>
    IController[] CreateControllers(Settings settings);
}
