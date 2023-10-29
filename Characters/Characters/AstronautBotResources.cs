using Common.ResourceClasses;

namespace Character.Characters;

public class AstronautBotResources : ModelResource
{
    private static readonly Lazy<AstronautBotResources> InternalInstance = new(() => new AstronautBotResources());

    public static AstronautBotResources Instance => InternalInstance.Value;

    public AstronautBotResources() : base(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Astronaut/model.dae"),
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Astronaut/texture-orange.png"),
        isAnimated: true)
    { }
}