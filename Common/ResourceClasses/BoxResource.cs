namespace Common.ResourceClasses;
public class BoxResource : ObjectResource
{
    private static readonly Lazy<BoxResource> InternalInstance = new(() => new BoxResource());

    public static BoxResource Instance => InternalInstance.Value;

    public BoxResource() : base(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/BoundingShapes/Box.dae"))
    { }
}
