using Hud.Shaders;
using OpenTK.Mathematics;

namespace Hyper.Shaders;
internal static class ShaderExtensionMethods
{
    public static void SetUp(this HudShader shader, float aspectRatio)
    {
        shader.Use();
        var projection = Matrix4.CreateOrthographic(aspectRatio, 1, -1.0f, 1.0f);
        shader.SetProjection(projection);
    }
}