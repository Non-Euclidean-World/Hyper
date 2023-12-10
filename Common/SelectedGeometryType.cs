namespace Common;

/// <summary>
/// Represents the selected geometry type.
/// </summary>
public enum SelectedGeometryType
{
    /// <summary>
    /// Euclidean geometry, representing flat space with no curvature.
    /// </summary>
    Euclidean,

    /// <summary>
    /// Hyperbolic geometry, representing negatively curved space.
    /// </summary>
    Hyperbolic,

    /// <summary>
    /// Spherical geometry, representing positively curved space.
    /// </summary>
    Spherical,

    /// <summary>
    /// Indicates no specific geometry is selected.
    /// </summary>
    None
}
