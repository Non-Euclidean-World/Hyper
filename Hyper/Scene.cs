using System.Reflection;
using Hyper.HUD;
using Hyper.MarchingCubes;
using Hyper.MathUtiils;
using Hyper.Meshes;
using Hyper.UserInput;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections.Concurrent;

namespace Hyper;

internal class Scene : IInputSubscriber
{
    public readonly ConcurrentDictionary<Guid, Chunk> _existingChunks;

    public readonly ChunkWorker _chunkWorker;

    public readonly List<LightSource> LightSources;

    public readonly List<Projectile> Projectiles;

    public readonly Camera Camera;

    public readonly HudManager Hud;

    public const float Scale = 0.1f;

    private readonly Shader _objectShader;

    private readonly Shader _lightSourceShader;


    public Scene(float aspectRatio)
    {
        _existingChunks = new ConcurrentDictionary<Guid, Chunk>();
        _chunkWorker = new ChunkWorker(_existingChunks);
        _chunkWorker.StartProcessing();
        
        LightSources = GetLightSources();
        Projectiles = new List<Projectile>();
        Camera = GetCamera(aspectRatio);
        Hud = new HudManager(aspectRatio);

        _objectShader = GetObjectShader();
        _lightSourceShader = GetLightSourceShader();

        RegisterCallbacks();
    }

    public void Render()
    {
        SetUpObjectShaderParams();

        foreach (var chunk in _existingChunks.Values)
        {
            chunk.CreateVertexArrayObject();
            chunk.Render(_objectShader, Scale, Camera.ReferencePointPosition);
        }

        foreach (var projectile in Projectiles)
        {
            projectile.CreateVertexArrayObject();
            projectile.Render(_objectShader, Scale, Camera.ReferencePointPosition);
        }

        SetUpLightingShaderParams();

        foreach (var light in LightSources)
        {
            light.CreateVertexArrayObject();
            light.Render(_lightSourceShader, Scale, Camera.ReferencePointPosition);
        }

        Hud.Render();
    }

    public void UpdateProjectiles(float time)
    {
        foreach (var projectile in Projectiles)
        {
            projectile.Update(time);
        }

        Projectiles.RemoveAll(x => x.IsDead);
    }

    private void SetUpObjectShaderParams()
    {
        _objectShader.Use();
        _objectShader.SetFloat("curv", Camera.Curve);
        _objectShader.SetFloat("anti", 1.0f);
        _objectShader.SetMatrix4("view", Camera.GetViewMatrix());
        _objectShader.SetMatrix4("projection", Camera.GetProjectionMatrix());
        _objectShader.SetInt("numLights", LightSources.Count);
        _objectShader.SetVector4("viewPos", GeomPorting.EucToCurved(Vector3.UnitY, Camera.Curve));

        for (int i = 0; i < LightSources.Count; i++)
        {
            _objectShader.SetVector3($"lightColor[{i}]", LightSources[i].Color);
            _objectShader.SetVector4($"lightPos[{i}]", GeomPorting.EucToCurved((LightSources[i].Position - Camera.ReferencePointPosition) * Scale, Camera.Curve));
        }
    }

    private void SetUpLightingShaderParams()
    {
        _lightSourceShader.Use();
        _lightSourceShader.SetFloat("curv", Camera.Curve);
        _lightSourceShader.SetFloat("anti", 1.0f);
        _lightSourceShader.SetMatrix4("view", Camera.GetViewMatrix());
        _lightSourceShader.SetMatrix4("projection", Camera.GetProjectionMatrix());
    }

    private static List<Chunk> GetChunks(ChunkFactory generator)
    {
        var chunks = new List<Chunk>
        {
            generator.GenerateChunk(new Vector3i(0, 0, 0)),
            generator.GenerateChunk(new Vector3i(Chunk.Size - 1, 0, 0)),
            generator.GenerateChunk(new Vector3i(0, 0, Chunk.Size - 1)),
            generator.GenerateChunk(new Vector3i(Chunk.Size - 1, 0, Chunk.Size - 1))
        };

        return chunks;
    }

    private List<LightSource> GetLightSources()
    {
        var lightSources = new List<LightSource> {
            new(CubeMesh.Vertices, new Vector3(10f, 7f + 10, 10f), new Vector3(1f, 1f, 1f)),
            new(CubeMesh.Vertices, new Vector3(4f, 7f + 10, 4f), new Vector3(0f, 1f, 0.5f)),
        };

        return lightSources;
    }

    private Camera GetCamera(float aspectRatio)
    {
        var camera = new Camera(aspectRatio, 0.01f, 100f, Scale)
        {
            ReferencePointPosition = (5f + 10) * Vector3.UnitY
        };

        return camera;
    }

    private static Shader GetObjectShader()
    {
        var shaderParams = new[]
        {
            ("Shaders/lighting_shader.vert", ShaderType.VertexShader),
            ("Shaders/lighting_shader.frag", ShaderType.FragmentShader)
        };

        return new Shader(shaderParams);
    }

    private static Shader GetLightSourceShader()
    {
        var shaderParams = new[]
        {
            ("Shaders/lighting_shader.vert", ShaderType.VertexShader),
            ("Shaders/light_source_shader.frag", ShaderType.FragmentShader)
        };
        return new Shader(shaderParams);
    }

    public void RegisterCallbacks()
    {
        Context context = Context.Instance;

        context.RegisterMouseButtons(new List<MouseButton> { MouseButton.Left, MouseButton.Right, MouseButton.Middle });

        context.RegisterUpdateFrameCallback((e) => UpdateProjectiles((float)e.Time));
        context.RegisterMouseButtonHeldCallback(MouseButton.Left, (e) =>
        {
            var position = Camera.ReferencePointPosition;

            foreach (var chunk in _existingChunks.Values)
            {
                if (chunk.Mine(position, (float)e.Time)) return;
            }
        });

        context.RegisterMouseButtonHeldCallback(MouseButton.Right, (e) =>
        {
            var position = Camera.ReferencePointPosition;
            foreach (var chunk in _existingChunks.Values)
            {
                if (chunk.Build(position, (float)e.Time)) return;
            }
        });

        context.RegisterMouseButtonDownCallback(MouseButton.Middle, () =>
        {
            var projectile = new Projectile(CubeMesh.Vertices, Camera.ReferencePointPosition, Camera.Front, 100f, 5f);
            Projectiles.Add(projectile);
        });
    }
}
