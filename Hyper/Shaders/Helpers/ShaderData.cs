using Common;
using Common.Meshes;
using Hyper.PlayerData;
using Hyper.PlayerData.Utils;
using Hyper.Shaders.DataTypes;
using OpenTK.Mathematics;

namespace Hyper.Shaders.Helpers;

/// <summary>
/// Collection of utility functions for setting up shader uniforms.
/// </summary>
internal static class ShaderData
{
    /// <summary>
    /// Sets up uniforms related to lighting
    /// </summary>
    /// <param name="shader">Shader to set the uniforms on</param>
    /// <param name="lightSources">List of point light sources</param>
    /// <param name="flashLights">List of spot lights</param>
    /// <param name="camera">Camera object</param>
    /// <param name="globalScale">Global scale parameter</param>
    public static void SetUpLighting(Shader shader, List<Lamp> lightSources, List<FlashLight> flashLights, Camera camera, float globalScale)
    {
        shader.SetInt("numPointLights", lightSources.Count);
        shader.SetInt("numSpotLights", flashLights.Where(x => x.Active).Count());
        shader.SetStructArray("pointLights", GetPointLights(lightSources, camera, globalScale));
        shader.SetStructArray("spotLights", GetSpotLights(flashLights, camera, globalScale));
        shader.SetVector4("viewPos", GeomPorting.EucToCurved(camera.ViewPosition, camera.Curve));
    }

    /// <summary>
    /// Sets up uniforms related to coordinate system transformations
    /// </summary>
    /// <param name="shader">Shader to set the uniforms on</param>
    /// <param name="camera">Camera object</param>
    public static void SetUpTransforms(Shader shader, Camera camera)
    {
        shader.SetFloat("curv", camera.Curve);
        shader.SetMatrix4("view", camera.GetViewMatrix());
        shader.SetMatrix4("projection", camera.GetProjectionMatrix());
    }

    private static PointLight[] GetPointLights(List<Lamp> lightSources, Camera camera, float globalScale)
    {
        return lightSources
            .Select(x =>
                new PointLight
                {
                    Position = GeomPorting.EucToCurved(
                        GeomPorting.CreateTranslationTarget(x.Position, camera.ReferencePointPosition, camera.Curve, globalScale),
                        camera.Curve),
                    Color = x.Color,
                    Ambient = x.Ambient,
                    Diffuse = x.Diffuse,
                    Specular = x.Specular,
                    Constant = x.Constant,
                    Linear = x.Linear,
                    Quadratic = x.Quadratic,
                })
            .ToArray();
    }

    private static SpotLight[] GetSpotLights(List<FlashLight> flashLights, Camera camera, float globalScale)
    {
        return flashLights
            .Where(x => x.Active)
            .Select(x =>
                new SpotLight
                {
                    Position = GeomPorting.EucToCurved(
                        GeomPorting.CreateTranslationTarget(x.Position, camera.ReferencePointPosition, camera.Curve, globalScale),
                        camera.Curve),
                    Color = x.Color,
                    Direction = new Vector4(x.Direction, 0) * Matrices.TranslationMatrix(GeomPorting.EucToCurved(GeomPorting.CreateTranslationTarget(x.Position, camera.ReferencePointPosition, camera.Curve, globalScale), camera.Curve), camera.Curve),
                    CutOff = x.CutOff,
                    OuterCutOff = x.OuterCutOff,
                    Ambient = x.Ambient,
                    Diffuse = x.Diffuse,
                    Specular = x.Specular,
                    Constant = x.Constant,
                    Linear = x.Linear,
                    Quadratic = x.Quadratic,
                })
            .ToArray();
    }
}
