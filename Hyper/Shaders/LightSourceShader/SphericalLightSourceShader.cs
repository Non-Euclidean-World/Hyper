using OpenTK.Mathematics;
using Player;

namespace Hyper.Shaders.LightSourceShader;
public class SphericalLightSourceShader : AbstractLightSourceShader
{
    private Vector3 _lowerSphereCenter;

    private SphericalLightSourceShader(float globalScale, Vector3 lowerSphereCenter)
        : base(globalScale)
    {
        _lowerSphereCenter = lowerSphereCenter;
    }

    public static SphericalLightSourceShader Create(float globalScale, Vector3 lowerSphereCenter)
    {
        return new SphericalLightSourceShader(globalScale, lowerSphereCenter);
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
