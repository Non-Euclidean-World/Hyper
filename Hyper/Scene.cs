using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Constraints;
using BepuUtilities;
using BepuUtilities.Memory;
using Hyper.Collisions;
using Hyper.Collisions.Bepu;
using Hyper.HUD;
using Hyper.MarchingCubes;
using Hyper.MathUtiils;
using Hyper.Meshes;
using Hyper.UserInput;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper;

internal class Scene : IInputSubscriber, IDisposable
{
    public readonly List<Chunk> Chunks;

    public readonly List<LightSource> LightSources;

    public readonly List<Projectile> Projectiles;

    public readonly CarMesh CarMesh;

    public readonly Camera Camera;

    public readonly HudManager Hud;

    public const float Scale = 0.1f;

    private readonly Shader _objectShader;

    private readonly Shader _lightSourceShader;

    private readonly ScalarFieldGenerator _scalarFieldGenerator;

    private readonly int _chunksPerSide = 2;

    private Simulation _simulation = null!;

    private BufferPool _bufferPool;

    private ThreadDispatcher _threadDispatcher;

    SimpleCarController _playerController;

    private readonly Vector3 _carInitialPosition;

    public Scene(float aspectRatio)
    {
        _scalarFieldGenerator = new ScalarFieldGenerator(1);
        ChunkFactory chunkFactory = new ChunkFactory(_scalarFieldGenerator);

        Chunks = GetChunks(chunkFactory);
        LightSources = GetLightSources(_chunksPerSide);
        Projectiles = new List<Projectile>();
        _carInitialPosition = new Vector3(-5, _scalarFieldGenerator.AvgElevation + 10, 10);
        CarMesh = new CarMesh(new Vector3(1.85f, 0.7f, 4.73f), 0.4f, 0.18f);

        Camera = GetCamera(aspectRatio);
        Hud = new HudManager(aspectRatio);

        _objectShader = GetObjectShader();
        _lightSourceShader = GetLightSourceShader();

        RegisterCallbacks();

        _bufferPool = new BufferPool();
        int targetThreadCount = int.Max(1, Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1);
        _threadDispatcher = new ThreadDispatcher(targetThreadCount);

        Initialize();
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

        CarMesh.Render(_objectShader, Scale, Camera.ReferencePointPosition);

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



    private List<Chunk> GetChunks(ChunkFactory generator)
    {
        return MakeSquare(_chunksPerSide, generator);
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

    public void RegisterCallbacks()
    {
        Context context = Context.Instance;

        context.RegisterMouseButtons(new List<MouseButton> { MouseButton.Left, MouseButton.Right, MouseButton.Middle });

        context.RegisterUpdateFrameCallback((e) => UpdateProjectiles((float)e.Time));
        context.RegisterUpdateFrameCallback((e) =>
        {
            _playerController.Update(_simulation, (float)e.Time, 0, 0f, false, false);
            _simulation.Timestep((float)e.Time, _threadDispatcher);
        });
        context.RegisterUpdateFrameCallback((e) =>
        {
            var carBody = new BodyReference(_playerController.Car.Body, _simulation.Bodies);
            var rearLeftWheel = new BodyReference(_playerController.Car.BackLeftWheel.Wheel, _simulation.Bodies);
            var rearRightWheel = new BodyReference(_playerController.Car.BackRightWheel.Wheel, _simulation.Bodies);
            var frontLeftWheel = new BodyReference(_playerController.Car.FrontLeftWheel.Wheel, _simulation.Bodies);
            var frontRightWheel = new BodyReference(_playerController.Car.FrontRightWheel.Wheel, _simulation.Bodies);
            CarMesh.Update(carBody.Pose, rearLeftWheel.Pose, rearRightWheel.Pose, frontLeftWheel.Pose, frontRightWheel.Pose);
        });

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

    private void Initialize()
    {
        var properties = new CollidableProperty<CarBodyProperties>();

        _simulation = Simulation.Create(_bufferPool, new CarCallbacks() { Properties = properties }, new PoseIntegratorCallbacks(new System.Numerics.Vector3(0, -10, 0)), new SolveDescription(6, 1));

        var builder = new CompoundBuilder(_bufferPool, _simulation.Shapes, 2);
        builder.Add(new Box(1.85f, 0.7f, 4.73f), RigidPose.Identity, 10);
        builder.Add(new Box(1.5f, 0.6f, 2.5f), new System.Numerics.Vector3(0, 0.65f, -1f), 0.5f);
        builder.BuildDynamicCompound(out var children, out var bodyInertia, out _);
        builder.Dispose();
        var bodyShape = new Compound(children);
        var bodyShapeIndex = _simulation.Shapes.Add(bodyShape);
        var wheelShape = new Cylinder(0.4f, .18f);
        var wheelInertia = wheelShape.ComputeInertia(0.25f);
        var wheelShapeIndex = _simulation.Shapes.Add(wheelShape);

        const float x = 0.9f;
        const float y = -0.1f;
        const float frontZ = 1.7f;
        const float backZ = -1.7f;
        const float wheelBaseWidth = x * 2;
        const float wheelBaseLength = frontZ - backZ;

        _playerController = new SimpleCarController(
            SimpleCar.Create(_simulation, properties, new System.Numerics.Vector3(_carInitialPosition.X, _carInitialPosition.Y, _carInitialPosition.Z), bodyShapeIndex, bodyInertia, 0.5f, wheelShapeIndex, wheelInertia, 2f,
                new System.Numerics.Vector3(x, y, frontZ), new System.Numerics.Vector3(-x, y, frontZ), new System.Numerics.Vector3(x, y, backZ), new System.Numerics.Vector3(-x, y, backZ), new System.Numerics.Vector3(0, -1, 0), 0.25f,
                new SpringSettings(5f, 0.7f), QuaternionEx.CreateFromAxisAngle(System.Numerics.Vector3.UnitZ, MathF.PI * 0.5f)),
                forwardSpeed: 75, forwardForce: 6, zoomMultiplier: 2, backwardSpeed: 30, backwardForce: 4, idleForce: 0.25f, brakeForce: 7, steeringSpeed: 1.5f, maximumSteeringAngle: MathF.PI * 0.23f,
                wheelBaseLength: wheelBaseLength, wheelBaseWidth: wheelBaseWidth, ackermanSteering: 1);

        foreach (var chunk in Chunks)
        {
            var mesh = MeshHelper.CreateMeshFromChunk(chunk, Scale, _bufferPool);
            var position = chunk.Position;
            _simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(position.X, position.Y, position.Z),
                QuaternionEx.Identity,
                _simulation.Shapes.Add(mesh)));
        }
    }

    bool _disposed;
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            _simulation.Dispose();
            _threadDispatcher.Dispose();
            _bufferPool.Clear();
        }
    }
}
