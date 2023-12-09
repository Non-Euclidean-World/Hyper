using Common.ResourceClasses;

namespace Character.Vehicles;
/// <summary>
/// A class that contains the shared resources for wheels of cars.
/// </summary>
public class CarWheelResource : ModelResource
{
    private static readonly Lazy<CarWheelResource> InternalInstance = new(() => new CarWheelResource());

    /// <summary>
    /// Instance of the class. Implemented as a singleton.
    /// </summary>
    public static CarWheelResource Instance => InternalInstance.Value;

    private CarWheelResource() : base(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Car/wheel.dae"),
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Car/wheelTexture.png"),
        isAnimated: false)
    { }
}
