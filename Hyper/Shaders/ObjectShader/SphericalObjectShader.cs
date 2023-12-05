using Character.LightSources;
using Common.Meshes;
using Hyper.PlayerData;
using OpenTK.Mathematics;

namespace Hyper.Shaders.ObjectShader;


internal class SphericalObjectShader : AbstractObjectShader
{
    private Vector3 _lowerSphereCenter;

    private SphericalObjectShader(Vector3 lowerSphereCenter)
        : base()
    {
        _lowerSphereCenter = lowerSphereCenter;
    }

    public static SphericalObjectShader Create(Vector3 lowerSphereCenter)
    {
        return new SphericalObjectShader(lowerSphereCenter);
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
