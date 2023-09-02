using Common;
using Common.Meshes;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Player;
using Player.Utils;

namespace Hyper.Shaders;
internal static class ShaderFactory
{
    public static Shader CreateObjectShader()
    {
        var shaderParams = new[]
        {
            ("Shaders/lighting_shader.vert", ShaderType.VertexShader),
            ("Shaders/lighting_shader.frag", ShaderType.FragmentShader)
        };

        return new Shader(shaderParams);
    }

    public static Shader CreateLightSourceShader()
    {
        var shaderParams = new[]
        {
            ("Shaders/lighting_shader.vert", ShaderType.VertexShader),
            ("Shaders/light_source_shader.frag", ShaderType.FragmentShader)
        };
        return new Shader(shaderParams);
    }

    public static Shader CreateModelShader()
    {
        var shader = new[]
        {
            ("../../../../Character/Shaders/model_shader.vert", ShaderType.VertexShader),
            ("../../../../Character/Shaders/model_shader.frag", ShaderType.FragmentShader)
        };
        return new Shader(shader);
    }

    public static Shader CreateHudShader()
    {
        var shader = new[]
        {
            (@"..\\..\\..\\..\\Hud\\bin\\Debug\\net7.0\\Shaders\\shader2d.vert", ShaderType.VertexShader),
            (@"..\\..\\..\\..\\Hud\\bin\\Debug\\net7.0\\Shaders\\shader2d.frag", ShaderType.FragmentShader)
        };

        return new Shader(shader);
    }

    public static void SetUpObjectShaderParams(Shader objectShader, Camera camera, List<LightSource> lightSources, float globalScale)
    {
        objectShader.Use();
        objectShader.SetFloat("curv", camera.Curve);
        objectShader.SetFloat("anti", 1.0f);
        objectShader.SetMatrix4("view", camera.GetViewMatrix());
        objectShader.SetMatrix4("projection", camera.GetProjectionMatrix());
        objectShader.SetInt("numLights", lightSources.Count);
        objectShader.SetVector4("viewPos", GeomPorting.EucToCurved(camera.ViewPosition, camera.Curve));

        objectShader.SetVector3Array("lightColor", lightSources.Select(x => x.Color).ToArray());
        objectShader.SetVector4Array("lightPos", lightSources.Select(x =>
            GeomPorting.EucToCurved((x.Position - camera.ReferencePointPosition) * globalScale, camera.Curve)).ToArray());
    }

    public static void SetUpLightingShaderParams(Shader lightSourceShader, Camera camera)
    {
        lightSourceShader.Use();
        lightSourceShader.SetFloat("curv", camera.Curve);
        lightSourceShader.SetFloat("anti", 1.0f);
        lightSourceShader.SetMatrix4("view", camera.GetViewMatrix());
        lightSourceShader.SetMatrix4("projection", camera.GetProjectionMatrix());
    }

    public static void SetUpCharacterShaderParams(Shader characterShader, Camera camera, List<LightSource> lightSources, float globalScale)
    {
        characterShader.Use();
        characterShader.SetFloat("curv", camera.Curve);
        characterShader.SetMatrix4("view", camera.GetViewMatrix());
        characterShader.SetMatrix4("projection", camera.GetProjectionMatrix());

        characterShader.SetInt("numLights", lightSources.Count);
        characterShader.SetVector4("viewPos", GeomPorting.EucToCurved(camera.ViewPosition, camera.Curve));
        characterShader.SetVector3Array("lightColor", lightSources.Select(x => x.Color).ToArray());
        characterShader.SetVector4Array("lightPos", lightSources.Select(x =>
            GeomPorting.EucToCurved((x.Position - camera.ReferencePointPosition) * globalScale, camera.Curve)).ToArray());
    }

    public static void SetUpHudShaderParams(Shader shader, float aspectRatio)
    {
        shader.Use();
        var projection = Matrix4.CreateOrthographic(aspectRatio, 1, -1.0f, 1.0f);
        shader.SetMatrix4("projection", projection);
    }
}
