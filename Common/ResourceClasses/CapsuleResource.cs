namespace Common.ResourceClasses;

/// <summary>
/// Represents a resource for a capsule-shaped model, inheriting from ModelResource.
/// </summary>
public class CapsuleResource : ModelResource
{
    private static readonly Lazy<CapsuleResource> InternalResource = new(() => new CapsuleResource());

    /// <summary>
    /// Gets the singleton instance of the CapsuleResource.
    /// </summary>
    public static CapsuleResource Instance => InternalResource.Value;

    private CapsuleResource() : base(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/BoundingShapes/Capsule.dae"))
    { }
}
