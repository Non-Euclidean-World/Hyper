using Hyper.Shaders.DataTypes;
using Hyper.Shaders.ModelShader;
using Hyper.Shaders.ObjectShader;
using OpenTK.Mathematics;

namespace Hyper.Controllers;
internal class SphericalSkyboxController : IController
{
    private readonly SphericalModelShader _modelShader;

    private readonly SphericalObjectShader _objectShader;
    public SphericalSkyboxController(SphericalModelShader modelShader, SphericalObjectShader objectShader)
    {
        _modelShader = modelShader;
        _objectShader = objectShader;
    }

    public void Dispose()
    {
    }

    public void Render()
    {
        DirectionalLight sunLight = new DirectionalLight
        {
            Ambient = 0.1f,
            Diffuse = 0.65f,
            Specular = 0.1f,
            Direction = new Vector4(Vector3.UnitY, 0)
        };

        _modelShader.SetStruct("sunLight", sunLight);
        _objectShader.SetStruct("sunLight", sunLight);
    }
}
