using Character.Shaders;
using Common;
using Common.Meshes;
using Hud.Shaders;
using OpenTK.Mathematics;
using Player;

namespace Hyper.Shaders;
internal static class ShaderExtensionMethods
{
    public static void SetUp(this ObjectShader objectShader, Camera camera, List<LightSource> lightSources, float globalScale, int sphere, Vector3 lowerSphereCenter)
    {
        objectShader.Use();
        objectShader.SetCurv(camera.Curve);
        objectShader.SetView(camera.GetViewMatrix());
        objectShader.SetProjection(camera.GetProjectionMatrix());
        objectShader.SetNumLights(lightSources.Count);
        objectShader.SetViewPos(GeomPorting.EucToCurved(camera.ViewPosition, camera.Curve));

        objectShader.SetLightColors(lightSources.Select(x => x.Color).ToArray());
        objectShader.SetLightPositions(lightSources.Select(x =>
            GeomPorting.EucToCurved(GeomPorting.CreateTranslationTarget(x.Position, camera.ReferencePointPosition, camera.Curve, globalScale), camera.Curve)).ToArray());

        objectShader.SetSphere(sphere);
        objectShader.SetLowerSphereCenter(lowerSphereCenter);
    }

    public static void SetUp(this LightSourceShader lightSourceShader, Camera camera, int sphere, Vector3 lowerSphereCenter)
    {
        lightSourceShader.Use();
        lightSourceShader.SetCurv(camera.Curve);
        lightSourceShader.SetView(camera.GetViewMatrix());
        lightSourceShader.SetProjection(camera.GetProjectionMatrix());

        lightSourceShader.SetSphere(sphere);
        lightSourceShader.SetLowerSphereCenter(lowerSphereCenter);
    }

    public static void SetUp(this ModelShader characterShader, Camera camera, List<LightSource> lightSources, float globalScale, int sphere, Vector3 lowerSphereCenter)
    {
        characterShader.Use();
        characterShader.SetCurv(camera.Curve);
        characterShader.SetView(camera.GetViewMatrix());
        characterShader.SetProjection(camera.GetProjectionMatrix());

        characterShader.SetNumLights(lightSources.Count);
        characterShader.SetViewPos(GeomPorting.EucToCurved(camera.ViewPosition, camera.Curve));
        characterShader.SetLightColors(lightSources.Select(x => x.Color).ToArray());
        characterShader.SetLightPositions(lightSources.Select(x =>
            GeomPorting.EucToCurved(GeomPorting.CreateTranslationTarget(x.Position, camera.ReferencePointPosition, camera.Curve, globalScale), camera.Curve)).ToArray());

        characterShader.SetSphere(sphere);
        characterShader.SetLowerSphereCenter(lowerSphereCenter);
    }

    public static void SetUp(this HudShader shader, float aspectRatio)
    {
        shader.Use();
        var projection = Matrix4.CreateOrthographic(aspectRatio, 1, -1.0f, 1.0f);
        shader.SetProjection(projection);
    }
}