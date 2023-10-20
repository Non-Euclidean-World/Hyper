namespace Common.ResourceClasses;

public class SphereResource : ObjectResource
{
    private static readonly Lazy<SphereResource> InternalInstance = new(() => new SphereResource());

    public static SphereResource Instance => InternalInstance.Value;

    public SphereResource() : base(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/BoundingShapes/sphere.dae"
        ))
    { }
}