using OpenTK.Mathematics;

namespace Hyper.Shaders.DataTypes;

/// <summary>
/// Shader-compatible spot light description
/// </summary>
internal struct SpotLight
{
    public Vector4 Position;
    public int SphereId;
    public Vector3 Color;

    public Vector4 Direction;
    public float CutOff;
    public float OuterCutOff;

    public float Constant;
    public float Linear;
    public float Quadratic;

    public Vector3 Ambient;
    public Vector3 Diffuse;
    public Vector3 Specular;
}
