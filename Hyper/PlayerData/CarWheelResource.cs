using Common.ResourceClasses;

namespace Hyper.PlayerData;
internal class CarWheelResource : TexturedObjectResource
{
    private static readonly Lazy<CarWheelResource> InternalInstance = new(() => new CarWheelResource());

    public static CarWheelResource Instance => InternalInstance.Value;

    protected CarWheelResource() : base(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Car/wheel.dae"),
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Car/wheelTexture.png"))
    { }
}
