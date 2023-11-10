using OpenTK.Mathematics;

namespace Hyper.Shaders.DataTypes;
internal struct EnvironmentInfo
{
    public Vector3 PrevPhaseSunLightColor;
    public Vector3 NextPhaseSunLightColor;
    public float PhaseT;
    public float PrevPhaseNightAmbient;
    public float NextPhaseNightAmbient;
}
