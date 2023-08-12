using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities;
using BepuUtilities.Memory;
/*using Hyper.Animation.Characters;
using Hyper.Animation.Characters.Cowboy;*/
using Hyper.Collisions;
using Hyper.Collisions.Bepu;
using Hyper.HUD;
using Hyper.MarchingCubes;
using Hyper.MathUtiils;
using Hyper.Meshes;
//using Hyper.PlayerData;
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

    public Camera Camera; //=> Player.Camera;

    //public readonly List<Character> Characters;
    // TODO these two fields should really be in one object
    CharacterControllers _characterControllers;

    Character _character;

    //public readonly Player Player;

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

    // BUG some assert fires if you do the following: 
    // make it so that the first thing that happens to the character is a collision with the car (before hitting any key)
    // make a jump -- you'll notice that the character gets stuck mid-air
    // make a move -- the assert fires
    // overall these are some very specific conditions, so I'll leave it for now
    private readonly Vector3 _characterInitialPosition;

    private readonly BufferPool _bufferPool;

    public Scene(float aspectRatio)
    {
        _scalarFieldGenerator = new ScalarFieldGenerator(1);
        ChunkFactory chunkFactory = new ChunkFactory(_scalarFieldGenerator);

        _chunks = GetChunks(chunkFactory);
        _lightSources = GetLightSources(_chunksPerSide);
        _projectiles = new List<Projectile>();
        //Characters = GetCharacters();
        //Player = new Player(GetCamera(aspectRatio));
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

        _simulationManager = new SimulationManager<NarrowPhaseCallbacks, PoseIntegratorCallbacks>(new NarrowPhaseCallbacks(_characterControllers) { Properties = _properties }, new PoseIntegratorCallbacks(new System.Numerics.Vector3(0, -10, 0)), new SolveDescription(6, 1), _bufferPool);

        _character = new Character(_characterControllers, TypingUtils.ToNumericsVector(_characterInitialPosition), new Capsule(0.5f, 1), 0.1f, 1, 20, 100, 6, 4, MathF.PI * 0.4f);



        _simpleCar = SimpleCar.CreateStandardCar(_simulationManager.Simulation, _simulationManager.BufferPool, _properties, TypingUtils.ToNumericsVector(_carInitialPosition));



        Camera = GetCamera(aspectRatio);

        foreach (var chunk in _chunks)
        {
            var mesh = MeshHelper.CreateMeshFromChunk(chunk, _scale, _simulationManager.BufferPool);
            var position = chunk.Position;
            _simulationManager.Simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(position.X, position.Y, position.Z),
                QuaternionEx.Identity,
                _simulationManager.Simulation.Shapes.Add(mesh)));
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

        SetUpLightingShaderParams();

        foreach (var light in _lightSources)
        {
            light.Render(_lightSourceShader, _scale, Camera.ReferencePointPosition);
        }

        SetUpCharacterShaderParams();

        /*foreach (var character in Characters)
        {
            character.Render(_characterShader, _scale, Camera.ReferencePointPosition);
        }*/

        //Player.Render(_characterShader, _scale, Camera.ReferencePointPosition);


        _simpleCar.Mesh.Render(_objectShader, _scale, Camera.ReferencePointPosition);

        _character.RenderCharacterMesh(_objectShader, _scale, Camera.ReferencePointPosition);

        Hud.Render();
    }

    public void UpdateProjectiles(float dt)
    {
        _projectiles.RemoveAll(x => x.IsDead);
        foreach (var projectile in _projectiles)
        {
            projectile.Update(_simulationManager.Simulation, dt);
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

    /*private static List<Character> GetCharacters()
    {
        var models = new List<Character>
            {
                new Cowboy(new Vector3(0, 20, 0), 1f)
            };

        return models;
    }*/

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
                TypingUtils.ToNumericsVector(movementDirection));

            _simulationManager.Simulation.Timestep((float)e.Time, _simulationManager.ThreadDispatcher);
        });

        context.RegisterMouseButtonHeldCallback(MouseButton.Left, (e) =>
        {
            var position = Camera.ReferencePointPosition;

            foreach (var chunk in _chunks)
            {
                if (chunk.Mine(position, (float)e.Time)) return;
            }
        });

        context.RegisterMouseButtonHeldCallback(MouseButton.Right, (e) =>
        {
            var position = Camera.ReferencePointPosition;

            foreach (var chunk in _chunks)
            {
                if (chunk.Build(position, (float)e.Time)) return;
            }
        });

        context.RegisterKeyDownCallback(Keys.P, () =>
        {
            var projectile = Projectile.CreateStandardProjectile(_simulationManager.Simulation, _properties, TypingUtils.ToNumericsVector(Camera.ReferencePointPosition), TypingUtils.ToNumericsVector(Camera.Front) * 15);
            _projectiles.Add(projectile);
        });
    }
}
