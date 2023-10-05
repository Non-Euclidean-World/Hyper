using Common.Meshes;
using OpenTK.Mathematics;
using Player;

namespace Hyper.Shaders.ObjectShader;
public class SphericalObjectShader : AbstractObjectShader
{
    private Vector3 _lowerSphereCenter;

    private SphericalObjectShader(float globalScale, Vector3 lowerSphereCenter)
        : base(globalScale)
    {
        _lowerSphereCenter = lowerSphereCenter;
    }

    public static SphericalObjectShader Create(float globalScale, Vector3 lowerSphereCenter)
    {
        return new SphericalObjectShader(globalScale, lowerSphereCenter);
    }

    public void SetLowerSphereCenter(Vector3 lowerSphereCenter) => SetVector3("lowerSphereCenter", lowerSphereCenter);

    public void SetSphere(int sphere) => SetInt("sphere", sphere);

    public override void SetUp(Camera camera, List<LightSource> lightSources, int sphere)
    {
        base.SetUp(camera, lightSources);

        SetSphere(sphere);
        SetLowerSphereCenter(_lowerSphereCenter);
    }
}
