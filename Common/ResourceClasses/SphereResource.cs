namespace Common.ResourceClasses;

/// <summary>
/// Represents a resource for a sphere-shaped model, inheriting from ModelResource.
/// </summary>
public class SphereResource : ModelResource
{
    private static readonly Lazy<SphereResource> InternalInstance = new(() => new SphereResource());

    /// <summary>
    /// Gets the singleton instance of the SphereResource.
    /// </summary>
    public static SphereResource Instance => InternalInstance.Value;

    private SphereResource() : base(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/BoundingShapes/sphere.dae"))
    { }
}