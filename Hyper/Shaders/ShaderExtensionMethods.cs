using Character.Shaders;
using Common.Meshes;
using Hud.Shaders;
using Hyper.PlayerData;
using Hyper.PlayerData.Utils;
using OpenTK.Mathematics;

namespace Hyper.Shaders;
internal static class ShaderExtensionMethods
{
    public static void SetUp(this ObjectShader objectShader, Camera camera, List<LightSource> lightSources, float globalScale)
    {
        objectShader.Use();
        objectShader.SetCurv(camera.Curve);
        objectShader.SetAnti(1.0f);
        objectShader.SetView(camera.GetViewMatrix());
        objectShader.SetProjection(camera.GetProjectionMatrix());
        objectShader.SetNumLights(lightSources.Count);
        objectShader.SetViewPos(GeomPorting.EucToCurved(camera.ViewPosition, camera.Curve));

        objectShader.SetLightColors(lightSources.Select(x => x.Color).ToArray());
        objectShader.SetLightPositions(lightSources.Select(x =>
            GeomPorting.EucToCurved((x.Position - camera.ReferencePointPosition) * globalScale, camera.Curve)).ToArray());
    }

    public static void SetUp(this LightSourceShader lightSourceShader, Camera camera)
    {
        lightSourceShader.Use();
        lightSourceShader.SetCurv(camera.Curve);
        lightSourceShader.SetAnti(1.0f);
        lightSourceShader.SetView(camera.GetViewMatrix());
        lightSourceShader.SetProjection(camera.GetProjectionMatrix());
    }

    public static void SetUp(this ModelShader characterShader, Camera camera, List<LightSource> lightSources, float globalScale)
    {
        characterShader.Use();
        characterShader.SetCurv(camera.Curve);
        characterShader.SetView(camera.GetViewMatrix());
        characterShader.SetProjection(camera.GetProjectionMatrix());

        characterShader.SetNumLights(lightSources.Count);
        characterShader.SetViewPos(GeomPorting.EucToCurved(camera.ViewPosition, camera.Curve));
        characterShader.SetLightColors(lightSources.Select(x => x.Color).ToArray());
        characterShader.SetLightPositions(lightSources.Select(x =>
            GeomPorting.EucToCurved((x.Position - camera.ReferencePointPosition) * globalScale, camera.Curve)).ToArray());
    }

    public static void SetUp(this HudShader shader, float aspectRatio)
    {
        shader.Use();
        var projection = Matrix4.CreateOrthographic(aspectRatio, 1, -1.0f, 1.0f);
        shader.SetProjection(projection);
    }
}
