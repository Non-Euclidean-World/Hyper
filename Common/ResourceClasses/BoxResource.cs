namespace Common.ResourceClasses;

/// <summary>
/// Represents a resource for a box-shaped model, inheriting from ModelResource.
/// </summary>
public class BoxResource : ModelResource
{
    private static readonly Lazy<BoxResource> InternalInstance = new(() => new BoxResource());

    /// <summary>
    /// Gets the singleton instance of the BoxResource.
    /// </summary>
    public static BoxResource Instance => InternalInstance.Value;

    private BoxResource() : base(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/BoundingShapes/Box.dae"))
    { }
}
