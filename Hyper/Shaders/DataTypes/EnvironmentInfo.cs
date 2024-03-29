﻿using OpenTK.Mathematics;

namespace Hyper.Shaders.DataTypes;

/// <summary>
/// Shader-compatible description of the environment
/// </summary>
internal struct EnvironmentInfo
{
    public Vector3 PrevPhaseSunLightColor;
    public Vector3 NextPhaseSunLightColor;
    public Vector3 PrevPhaseMoonLightColor;
    public Vector3 NextPhaseMoonLightColor;
    public float PhaseT;
    public float PrevPhaseNightAmbient;
    public float NextPhaseNightAmbient;
}
