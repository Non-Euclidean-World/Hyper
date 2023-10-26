namespace Character.Characters;

public class AstronautResources : ModelResources
{
    private static readonly Lazy<AstronautResources> InternalInstance = new(() => new AstronautResources());

    public static AstronautResources Instance => InternalInstance.Value;

    public AstronautResources() : base(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Astronaut/model.dae"),
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Astronaut/texture.png"))
    { }
}