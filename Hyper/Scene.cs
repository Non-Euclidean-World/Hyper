using Hyper.Animation;
using Hyper.Animation.Characters;
using Hyper.Animation.Characters.Cowboy;
using Hyper.HUD;
using Hyper.MarchingCubes;
using Hyper.MathUtiils;
using Hyper.Meshes;
using Hyper.PlayerData;
using Hyper.UserInput;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper;

internal class Scene : IInputSubscriber
{
    public readonly List<Chunk> Chunks;

    public readonly List<LightSource> LightSources;

    public readonly List<Projectile> Projectiles;
        
    public readonly List<Character> Characters;

    public readonly Player Player;

    public Camera Camera => Player.Camera;

    public readonly HudManager Hud;

    public const float Scale = 0.1f;

    private readonly Shader _objectShader;

    private readonly Shader _lightSourceShader;

    private readonly Shader _characterShader;

    private readonly ScalarFieldGenerator _scalarFieldGenerator;

    public Scene(float aspectRatio)
    {
        _scalarFieldGenerator = new ScalarFieldGenerator(1);
        ChunkFactory chunkFactory = new ChunkFactory(_scalarFieldGenerator);

        Chunks = GetChunks(chunkFactory);
        LightSources = GetLightSources();
        Projectiles = new List<Projectile>();
        Characters = GetCharacters();
        Player = new Player(GetCamera(aspectRatio));
        Hud = new HudManager(aspectRatio);

        _objectShader = GetObjectShader();
        _lightSourceShader = GetLightSourceShader();
        _characterShader = GetModelShader();

        RegisterCallbacks();
    }

    public void Render()
    {
        SetUpObjectShaderParams();

        foreach (var chunk in Chunks)
        {
            chunk.Render(_objectShader, Scale, Camera.ReferencePointPosition);
        }

        foreach (var projectile in Projectiles)
        {
            projectile.Render(_objectShader, Scale, Camera.ReferencePointPosition);
        }

        SetUpLightingShaderParams();

        foreach (var light in LightSources)
        {
            light.Render(_lightSourceShader, Scale, Camera.ReferencePointPosition);
        }
            
        SetUpCharacterShaderParams();
            
        foreach (var character in Characters)
        {
            character.Render(_characterShader, Scale, Camera.ReferencePointPosition);
        }
        
        Player.Render(_characterShader, Scale, Camera.ReferencePointPosition);

        Hud.Render();
    }

    private void UpdateProjectiles(float time)
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

        _objectShader.SetVector3Array("lightColor", LightSources.Select(x => x.Color).ToArray());
        _objectShader.SetVector4Array("lightPos", LightSources.Select(x =>
            GeomPorting.EucToCurved((x.Position - Camera.ReferencePointPosition) * Scale, Camera.Curve)).ToArray());
    }

    private void SetUpLightingShaderParams()
    {
        _lightSourceShader.Use();
        _lightSourceShader.SetFloat("curv", Camera.Curve);
        _lightSourceShader.SetFloat("anti", 1.0f);
        _lightSourceShader.SetMatrix4("view", Camera.GetViewMatrix());
        _lightSourceShader.SetMatrix4("projection", Camera.GetProjectionMatrix());
    }

    private void SetUpCharacterShaderParams()
    {
        _characterShader.Use();
        _characterShader.SetFloat("curv", Camera.Curve);
        _characterShader.SetMatrix4("view", Camera.GetViewMatrix());
        _characterShader.SetMatrix4("projection", Camera.GetProjectionMatrix());
        
        _characterShader.SetInt("numLights", LightSources.Count);
        _characterShader.SetVector4("viewPos", GeomPorting.EucToCurved(Vector3.UnitY, Camera.Curve));
        _characterShader.SetVector3Array("lightColor", LightSources.Select(x => x.Color).ToArray());
        _characterShader.SetVector4Array("lightPos", LightSources.Select(x =>
            GeomPorting.EucToCurved((x.Position - Camera.ReferencePointPosition) * Scale, Camera.Curve)).ToArray());
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
            new(CubeMesh.Vertices, new Vector3(10f, 7f + _scalarFieldGenerator.AvgElevation, 10f), new Vector3(1f, 1f, 1f)),
            new(CubeMesh.Vertices, new Vector3(4f, 7f + _scalarFieldGenerator.AvgElevation, 4f), new Vector3(0f, 1f, 0.5f)),
        };

        return lightSources;
    }

    private static List<Character> GetCharacters()
    {
        var models = new List<Character>
            {
                new Cowboy(new Vector3(0, 20, 0), 1f)
            };

        return models;
    }

    private Camera GetCamera(float aspectRatio)
    {
        var camera = new Camera(aspectRatio, 0.01f, 100f, Scale)
        {
            ReferencePointPosition = (5f + _scalarFieldGenerator.AvgElevation) * Vector3.UnitY
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
    
    private static Shader GetModelShader()
    {
        var shader = new[]
        {
            ("Animation/Shaders/model_shader.vert", ShaderType.VertexShader),
            ("Animation/Shaders/model_shader.frag", ShaderType.FragmentShader)
        };
        return new Shader(shader);
    }

    public void RegisterCallbacks()
    {
        Context context = Context.Instance;

        context.RegisterMouseButtons(new List<MouseButton> { MouseButton.Left, MouseButton.Right, MouseButton.Middle });

        context.RegisterUpdateFrameCallback((e) => UpdateProjectiles((float)e.Time));
        context.RegisterMouseButtonHeldCallback(MouseButton.Left, (e) =>
        {
            var position = Camera.ReferencePointPosition;

            foreach (var chunk in Chunks)
            {
                if (chunk.Mine(position, (float)e.Time)) return;
            }
        });

        context.RegisterMouseButtonHeldCallback(MouseButton.Right, (e) =>
        {
            var position = Camera.ReferencePointPosition;

            foreach (var chunk in Chunks)
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
