using Common.ResourceClasses;

namespace Character.Characters;
/// <summary>
/// A class that contains the shared resources for astronaut bots.
/// </summary>
public class AstronautBotResources : ModelResource
{
    private static readonly Lazy<AstronautBotResources> InternalInstance = new(() => new AstronautBotResources());

    /// <summary>
    /// Instance of the class. Implemented as a singleton.
    /// </summary>
    public static AstronautBotResources Instance => InternalInstance.Value;

    private AstronautBotResources() : base(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Astronaut/model.dae"),
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Astronaut/texture-orange.png"),
        isAnimated: true)
    { }
}