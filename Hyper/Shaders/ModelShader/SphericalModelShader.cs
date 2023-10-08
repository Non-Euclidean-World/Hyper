using Common.Meshes;
using Hyper.PlayerData;
using OpenTK.Mathematics;

namespace Hyper.Shaders.ModelShader;
internal class SphericalModelShader : AbstractModelShader
{
    private Vector3 _lowerSphereCenter;

    private SphericalModelShader(float globalScale, Vector3 lowerSphereCenter)
        : base(globalScale)
    {
        _lowerSphereCenter = lowerSphereCenter;
    }

    public static SphericalModelShader Create(float globalScale, Vector3 lowerSphereCenter)
    {
        return new SphericalModelShader(globalScale, lowerSphereCenter);
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
