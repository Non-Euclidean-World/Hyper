using Hyper.PlayerData;
using OpenTK.Mathematics;

namespace Hyper.Shaders.LightSourceShader;


internal class SphericalLightSourceShader : AbstractLightSourceShader
{
    private Vector3 _lowerSphereCenter;

    private SphericalLightSourceShader(Vector3 lowerSphereCenter)
        : base()
    {
        _lowerSphereCenter = lowerSphereCenter;
    }

    public static SphericalLightSourceShader Create(Vector3 lowerSphereCenter)
    {
        return new SphericalLightSourceShader(lowerSphereCenter);
    }

    public void SetLowerSphereCenter(Vector3 lowerSphereCenter) => SetVector3("lowerSphereCenter", lowerSphereCenter);

    public void SetSphere(int sphere) => SetInt("sphere", sphere);

    public override void SetUp(Camera camera, int sphere = 0)
    {
        base.SetUp(camera, sphere);

        SetSphere(sphere);
        SetLowerSphereCenter(_lowerSphereCenter);
    }
}
