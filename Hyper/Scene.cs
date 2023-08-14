using BepuPhysics;
using BepuUtilities;
using BepuUtilities.Memory;
using Hyper.Collisions;
using Hyper.Collisions.Bepu;
using Hyper.HUD;
using Hyper.MarchingCubes;
using Hyper.MathUtiils;
using Hyper.Meshes;
using Hyper.TypingUtils;
using Hyper.UserInput;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace Hyper;

internal class Scene : IInputSubscriber
{
    private readonly List<Chunk> _chunks;

    private readonly List<LightSource> _lightSources;

    private readonly List<Projectile> _projectiles;

    public Camera Camera { get; set; }

    // TODO these two fields should really be in one object
    private readonly CharacterControllers _characterControllers;

    private readonly Character _character;

    public HudManager Hud { get; private set; }

    private readonly float _scale = 0.1f;

    private readonly Shader _objectShader;

    private readonly Shader _lightSourceShader;

    private readonly Shader _characterShader;

    private readonly ScalarFieldGenerator _scalarFieldGenerator;

    private readonly int _chunksPerSide = 2;

    private readonly SimulationManager<NarrowPhaseCallbacks, PoseIntegratorCallbacks> _simulationManager;

    private readonly CollidableProperty<SimulationProperties> _properties;

    private readonly SimpleCar _simpleCar;

    private readonly Vector3 _carInitialPosition;

    private readonly Vector3 _characterInitialPosition;

    private readonly BufferPool _bufferPool;

    public Scene(float aspectRatio)
    {
        _scalarFieldGenerator = new ScalarFieldGenerator(1);
        ChunkFactory chunkFactory = new ChunkFactory(_scalarFieldGenerator);

        _chunks = GetChunks(chunkFactory);
        _lightSources = GetLightSources(_chunksPerSide);
        _projectiles = new List<Projectile>();

        _carInitialPosition = new Vector3(5, _scalarFieldGenerator.AvgElevation + 8, 12);
        _characterInitialPosition = new Vector3(0, _scalarFieldGenerator.AvgElevation + 8, 15);

        Hud = new HudManager(aspectRatio);

        _objectShader = GetObjectShader();
        _lightSourceShader = GetLightSourceShader();
        _characterShader = GetModelShader();

        RegisterCallbacks();

        _bufferPool = new BufferPool();

        _properties = new CollidableProperty<SimulationProperties>();

        _characterControllers = new CharacterControllers(_bufferPool);

        _simulationManager = new SimulationManager<NarrowPhaseCallbacks, PoseIntegratorCallbacks>(new NarrowPhaseCallbacks(_characterControllers, _properties),
            new PoseIntegratorCallbacks(new System.Numerics.Vector3(0, -10, 0)),
            new SolveDescription(6, 1), _bufferPool);

        _character = new Character(_characterControllers, Conversions.ToNumericsVector(_characterInitialPosition), 0.1f, 1, 20, 100, 6, 4, MathF.PI * 0.4f);

        _simpleCar = SimpleCar.CreateStandardCar(_simulationManager.Simulation, _simulationManager.BufferPool, _properties, Conversions.ToNumericsVector(_carInitialPosition));

        Camera = GetCamera(aspectRatio);

        // TODO this is really awful
        foreach (var chunk in _chunks)
        {
            var mesh = MeshHelper.CreateMeshFromChunk(chunk, _simulationManager.BufferPool);
            var position = chunk.Position;
            chunk.Shape = _simulationManager.Simulation.Shapes.Add(mesh);
            chunk.Handle = _simulationManager.Simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(position.X, position.Y, position.Z),
                QuaternionEx.Identity,
                chunk.Shape));
        }
    }

    public void Render()
    {
        SetUpObjectShaderParams();

        foreach (var chunk in _chunks)
        {
            chunk.Render(_objectShader, _scale, Camera.ReferencePointPosition);
        }

        foreach (var projectile in _projectiles)
        {
            projectile.Mesh.Render(_objectShader, _scale, Camera.ReferencePointPosition);
        }

        _simpleCar.Mesh.Render(_objectShader, _scale, Camera.ReferencePointPosition);

        SetUpLightingShaderParams();

        foreach (var light in _lightSources)
        {
            light.Render(_lightSourceShader, _scale, Camera.ReferencePointPosition);
        }

        SetUpCharacterShaderParams();

        _character.RenderCharacterMesh(_characterShader,
#if BOUNDING_BOXES
            _objectShader,
#endif
            _scale, Camera.ReferencePointPosition, Camera.FirstPerson);

        Hud.Render();
    }

    public void UpdateProjectiles(float dt)
    {
        _projectiles.RemoveAll(x => x.IsDead);
        foreach (var projectile in _projectiles)
        {
            projectile.Update(_simulationManager.Simulation, dt, _simulationManager.BufferPool);
        }
    }

    private void SetUpObjectShaderParams()
    {
        _objectShader.Use();
        _objectShader.SetFloat("curv", Camera.Curve);
        _objectShader.SetFloat("anti", 1.0f);
        _objectShader.SetMatrix4("view", Camera.GetViewMatrix());
        _objectShader.SetMatrix4("projection", Camera.GetProjectionMatrix());
        _objectShader.SetInt("numLights", _lightSources.Count);
        _objectShader.SetVector4("viewPos", GeomPorting.EucToCurved(Vector3.UnitY, Camera.Curve));

        _objectShader.SetVector3Array("lightColor", _lightSources.Select(x => x.Color).ToArray());
        _objectShader.SetVector4Array("lightPos", _lightSources.Select(x =>
            GeomPorting.EucToCurved((x.Position - Camera.ReferencePointPosition) * _scale, Camera.Curve)).ToArray());
    }

    private void SetUpLightingShaderParams()
    {
        _lightSourceShader.Use();
        _lightSourceShader.SetFloat("curv", Camera.Curve);
        _lightSourceShader.SetFloat("anti", 1.0f);
        _lightSourceShader.SetMatrix4("view", Camera.GetViewMatrix());
        _lightSourceShader.SetMatrix4("projection", Camera.GetProjectionMatrix());
    }


    private List<Chunk> GetChunks(ChunkFactory generator)
    {
        return MakeSquare(_chunksPerSide, generator);
    }

    private void SetUpCharacterShaderParams()
    {
        _characterShader.Use();
        _characterShader.SetFloat("curv", Camera.Curve);
        _characterShader.SetMatrix4("view", Camera.GetViewMatrix());
        _characterShader.SetMatrix4("projection", Camera.GetProjectionMatrix());

        _characterShader.SetInt("numLights", _lightSources.Count);
        _characterShader.SetVector4("viewPos", GeomPorting.EucToCurved(Vector3.UnitY, Camera.Curve));
        _characterShader.SetVector3Array("lightColor", _lightSources.Select(x => x.Color).ToArray());
        _characterShader.SetVector4Array("lightPos", _lightSources.Select(x =>
            GeomPorting.EucToCurved((x.Position - Camera.ReferencePointPosition) * _scale, Camera.Curve)).ToArray());
    }

    private static List<Chunk> MakeSquare(int chunksPerSide, ChunkFactory generator)
    {
        if (chunksPerSide % 2 != 0)
            throw new ArgumentException("# of chunks/side must be even");

        List<Chunk> chunks = new List<Chunk>();
        for (int x = -chunksPerSide / 2; x < chunksPerSide / 2; x++)
        {
            for (int y = -chunksPerSide / 2; y < chunksPerSide / 2; y++)
            {
                int offset = Chunk.Size - 1;

                chunks.Add(generator.GenerateChunk(new Vector3i(offset * x, 0, offset * y)));
            }
        }

        return chunks;
    }

    private List<LightSource> GetLightSources(int chunksPerSide)
    {
        if (chunksPerSide % 2 != 0)
            throw new ArgumentException("# of chunks/side must be even");

        List<LightSource> lightSources = new List<LightSource>();
        for (int x = -chunksPerSide / 2; x < chunksPerSide / 2; x++)
        {
            for (int y = -chunksPerSide / 2; y < chunksPerSide / 2; y++)
            {
                if (x % 2 == 0 && y % 2 == 0)
                    continue;

                int offset = Chunk.Size - 1;

                lightSources.Add(new LightSource(CubeMesh.Vertices, new Vector3(offset * x, _scalarFieldGenerator.AvgElevation + 10f, offset * y), new Vector3(1, 1, 1)));
            }
        }

        return lightSources;
    }

    private Camera GetCamera(float aspectRatio)
    {
        var camera = new Camera(aspectRatio, 0.01f, 100f, _scale)
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

        context.RegisterMouseButtons(new List<MouseButton> { MouseButton.Left, MouseButton.Right });
        context.RegisterKeys(new List<Keys> { Keys.Backspace, Keys.P, Keys.LeftShift, Keys.Space });
        context.RegisterUpdateFrameCallback((e) => UpdateProjectiles((float)e.Time));
        context.RegisterUpdateFrameCallback((e) =>
        {
            // TODO commented out until we have context switching
            /*float steeringSum = 0;
            if (context.HeldKeys[Keys.A]) steeringSum += 1;
            if (context.HeldKeys[Keys.D]) steeringSum -= 1;
            float targetSpeedFraction = context.HeldKeys[Keys.W] ? 1f : context.HeldKeys[Keys.S] ? -1f : 0;*/
            _simpleCar.Update(_simulationManager.Simulation, (float)e.Time, 0/*steeringSum*/, 0f/*targetSpeedFraction*/, false, false /*context.HeldKeys[Keys.Backspace]*/);

            Vector2 movementDirection = default;
            if (context.HeldKeys[Keys.W])
            {
                movementDirection = new Vector2(0, 1);
            }
            if (context.HeldKeys[Keys.S])
            {
                movementDirection += new Vector2(0, -1);
            }
            if (context.HeldKeys[Keys.A])
            {
                movementDirection += new Vector2(-1, 0);
            }
            if (context.HeldKeys[Keys.D])
            {
                movementDirection += new Vector2(1, 0);
            }
            _character.UpdateCharacterGoals(_simulationManager.Simulation, Camera, (float)e.Time,
                tryJump: context.HeldKeys[Keys.Space], sprint: context.HeldKeys[Keys.LeftShift],
                Conversions.ToNumericsVector(movementDirection));

            _simulationManager.Simulation.Timestep((float)e.Time, _simulationManager.ThreadDispatcher);
        });

        context.RegisterMouseButtonHeldCallback(MouseButton.Left, (e) =>
        {
            foreach (var chunk in _chunks)
            {
                if (chunk.Mine(Conversions.ToOpenTKVector(GetCharacterRay(1)), (float)e.Time))
                {
                    // interestingly this doesn't kill the performance
                    _simulationManager.Simulation.Shapes.RemoveAndDispose(chunk.Shape, _simulationManager.BufferPool);
                    var mesh = MeshHelper.CreateMeshFromChunk(chunk, _simulationManager.BufferPool);
                    var position = chunk.Position;
                    chunk.Shape = _simulationManager.Simulation.Shapes.Add(mesh);
                    _simulationManager.Simulation.Statics[chunk.Handle].SetShape(chunk.Shape);
                    return;
                }
            }
        });

        context.RegisterMouseButtonHeldCallback(MouseButton.Right, (e) =>
        {
            foreach (var chunk in _chunks)
            {
                if (chunk.Build(Conversions.ToOpenTKVector(GetCharacterRay(3)), (float)e.Time))
                {
                    // interestingly this doesn't kill the performance
                    _simulationManager.Simulation.Shapes.RemoveAndDispose(chunk.Shape, _simulationManager.BufferPool);
                    var mesh = MeshHelper.CreateMeshFromChunk(chunk, _simulationManager.BufferPool);
                    var position = chunk.Position;
                    chunk.Shape = _simulationManager.Simulation.Shapes.Add(mesh);
                    _simulationManager.Simulation.Statics[chunk.Handle].SetShape(chunk.Shape);
                    return;
                }
            }
        });

        context.RegisterKeyDownCallback(Keys.P, () =>
        {
            var projectile = Projectile.CreateStandardProjectile(_simulationManager.Simulation,
                _properties,
                GetCharacterRay(2),
                Conversions.ToNumericsVector(Camera.Front) * 15);
            _projectiles.Add(projectile);
        });
    }

    private System.Numerics.Vector3 GetCharacterRay(float length)
        => _character.Player.Character.RigidPose.Position
                + Conversions.ToNumericsVector(Camera.Front * length);
}
