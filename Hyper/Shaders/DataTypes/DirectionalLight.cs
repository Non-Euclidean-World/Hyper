using OpenTK.Mathematics;

namespace Hyper.Shaders.DataTypes;
internal struct DirectionalLight
{
    public Vector4 Direction;

    public float Ambient;
    public float Diffuse;
    public float Specular;
}
