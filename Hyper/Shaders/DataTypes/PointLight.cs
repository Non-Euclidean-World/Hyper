﻿using OpenTK.Mathematics;

namespace Hyper.Shaders.DataTypes;

/// <summary>
/// Point light source description
/// </summary>
public struct PointLight
{
    public Vector4 Position;
    public Vector3 Color;

    public Vector3 Ambient;
    public Vector3 Diffuse;
    public Vector3 Specular;

    public float Constant;
    public float Linear;
    public float Quadratic;
}
