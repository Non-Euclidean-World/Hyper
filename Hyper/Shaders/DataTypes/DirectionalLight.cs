using OpenTK.Mathematics;

namespace Hyper.Shaders.DataTypes;

/// <summary>
/// Shader-compatible description of a directional light source
/// </summary>
internal struct DirectionalLight
{
    public Vector4 Direction;

    public float Ambient;
    public float Diffuse;
    public float Specular;
}
