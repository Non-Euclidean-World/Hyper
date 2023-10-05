using Common.Meshes;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Player;

namespace Hyper.Shaders;
internal class SphericalModelShader : AbstractModelShader
{
    private Vector3 _lowerSphereCenter;

    private SphericalModelShader((string path, ShaderType shaderType)[] shaders, float globalScale, Vector3 lowerSphereCenter)
        : base(shaders, globalScale)
    {
        _lowerSphereCenter = lowerSphereCenter;
    }

    public static SphericalModelShader Create(float globalScale, Vector3 lowerSphereCenter)
    {
        var shader = new[]
        {
            ("Shaders/model_shader.vert", ShaderType.VertexShader),
            ("Shaders/model_shader.frag", ShaderType.FragmentShader)
        };

        return new SphericalModelShader(shader, globalScale, lowerSphereCenter);
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
