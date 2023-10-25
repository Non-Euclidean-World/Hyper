using Common.ResourceClasses;

namespace Character.Characters;

public class CowboyResources : ModelResource
{
    private static readonly Lazy<CowboyResources> InternalInstance = new(() => new CowboyResources());

    public static CowboyResources Instance => InternalInstance.Value;

    public CowboyResources() : base(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Cowboy/model.dae"),
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Cowboy/texture.png"),
        isAnimated: true)
    { }
}