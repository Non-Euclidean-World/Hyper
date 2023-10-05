using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Player;

namespace Hyper.Shaders;
public class SphericalLightSourceShader : AbstractLightSourceShader
{
    private Vector3 _lowerSphereCenter;

    private SphericalLightSourceShader((string path, ShaderType shaderType)[] shaders, float globalScale, Vector3 lowerSphereCenter)
        : base(shaders, globalScale)
    {
        _lowerSphereCenter = lowerSphereCenter;
    }

    public static SphericalLightSourceShader Create(float globalScale, Vector3 lowerSphereCenter)
    {
        var shader = new[]
        {
            ("Shaders/lighting_shader.vert", ShaderType.VertexShader),
            ("Shaders/light_source_shader.frag", ShaderType.FragmentShader)
        };

        return new SphericalLightSourceShader(shader, globalScale, lowerSphereCenter);
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
