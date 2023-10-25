using Common.ResourceClasses;

namespace Character.Vehicles;
public class CarWheelResource : ModelResource
{
    private static readonly Lazy<CarWheelResource> InternalInstance = new(() => new CarWheelResource());

    public static CarWheelResource Instance => InternalInstance.Value;

    protected CarWheelResource() : base(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Car/wheel.dae"),
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Car/wheelTexture.png"),
        isAnimated: false)
    { }
}
