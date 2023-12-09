using Common.ResourceClasses;

namespace Character.Vehicles;
/// <summary>
/// A class that contains the shared resources for cars.
/// </summary>
public class CarBodyResource : ModelResource
{
    private static readonly Lazy<CarBodyResource> InternalResource = new(() => new CarBodyResource());
    
    /// <summary>
    /// Instance of the class. Implemented as a singleton.
    /// </summary>
    public static CarBodyResource Instance => InternalResource.Value;

    private CarBodyResource() : base(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Car/body.dae"),
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Car/bodyTexture.png"),
        isAnimated: false)
    { }
}
