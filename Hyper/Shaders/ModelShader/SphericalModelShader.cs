using Character.LightSources;
using Common.Meshes;
using Hyper.PlayerData;
using OpenTK.Mathematics;

namespace Hyper.Shaders.ModelShader;
internal class SphericalModelShader : AbstractModelShader
{
    private Vector3 _lowerSphereCenter;

    private SphericalModelShader(Vector3 lowerSphereCenter)
        : base()
    {
        _lowerSphereCenter = lowerSphereCenter;
    }

    public static SphericalModelShader Create(Vector3 lowerSphereCenter)
    {
        return new SphericalModelShader(lowerSphereCenter);
    }

    public void SetLowerSphereCenter(Vector3 lowerSphereCenter) => SetVector3("lowerSphereCenter", lowerSphereCenter);

    public void SetSphere(int sphere) => SetInt("sphere", sphere);

    public override void SetUp(Camera camera, List<Lamp> lightSources, List<FlashLight> flashLights, float shininess, float globalScale, int sphere)
    {
        base.SetUp(camera, lightSources, flashLights, shininess, globalScale);

        SetSphere(sphere);
        SetLowerSphereCenter(_lowerSphereCenter);
    }
}
