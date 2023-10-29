namespace Character.Characters;

public class OrangeAstronautResources : ModelResources
{
    private static readonly Lazy<OrangeAstronautResources> InternalInstance = new(() => new OrangeAstronautResources());

    public static OrangeAstronautResources Instance => InternalInstance.Value;

    public OrangeAstronautResources() : base(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Astronaut/model.dae"),
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Astronaut/texture-orange.png"))
    { }
}