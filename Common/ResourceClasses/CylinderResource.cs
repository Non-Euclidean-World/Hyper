namespace Common.ResourceClasses;

/// <summary>
/// Represents a resource for a cylinder-shaped model, inheriting from ModelResource.
/// </summary>
public class CylinderResource : ModelResource
{
    private static readonly Lazy<CylinderResource> InternalInstance = new(() => new CylinderResource());

    /// <summary>
    /// Gets the singleton instance of the CylinderResource.
    /// </summary>
    public static CylinderResource Instance => InternalInstance.Value;

    private CylinderResource() : base(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/BoundingShapes/Cylinder.dae"))
    { }
}
