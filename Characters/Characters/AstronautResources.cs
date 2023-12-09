using Common.ResourceClasses;

namespace Character.Characters;
/// <summary>
/// A class that contains resources for the player model.
/// </summary>
public class AstronautResources : ModelResource
{
    private static readonly Lazy<AstronautResources> InternalInstance = new(() => new AstronautResources());

    /// <summary>
    /// Instance of the class. Implemented as a singleton.
    /// </summary>
    public static AstronautResources Instance => InternalInstance.Value;

    private AstronautResources() : base(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Astronaut/model.dae"),
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Astronaut/texture.png"),
        isAnimated: true)
    { }
}