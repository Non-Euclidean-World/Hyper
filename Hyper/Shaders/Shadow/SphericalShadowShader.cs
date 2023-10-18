using OpenTK.Mathematics;

namespace Hyper.Shaders.Shadow;

public class SphericalShadowShader : AbstractShadowShader
{
    private Vector3 _lowerSphereCenter;

    private SphericalShadowShader(float globalScale, Vector3 lowerSphereCenter)
        : base(globalScale)
    {
        _lowerSphereCenter = lowerSphereCenter;
    }

    public static SphericalShadowShader Create(float globalScale, Vector3 lowerSphereCenter)
    {
        return new SphericalShadowShader(globalScale, lowerSphereCenter);
    }
}