namespace Common;
public class CylinderResource : ObjectResource
{
    private static readonly Lazy<CylinderResource> InternalInstance = new(() => new CylinderResource());

    public static CylinderResource Instance => InternalInstance.Value;

    public CylinderResource() : base(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/BoundingShapes/Cylinder.dae"
        ))
    { }
}
