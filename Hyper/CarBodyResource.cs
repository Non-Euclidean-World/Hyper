using Common.ResourceClasses;

namespace Hyper;
internal class CarBodyResource : TexturedObjectResource
{
    private static readonly Lazy<CarBodyResource> InternalResource = new(() => new CarBodyResource());

    public static CarBodyResource Instance => InternalResource.Value;

    protected CarBodyResource() : base(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Car/body.dae"),
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Car/bodyTexture.png"))
    { }
}
