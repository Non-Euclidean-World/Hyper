using System.Diagnostics;
using BepuPhysics;
using BepuUtilities.Memory;
using Character.GameEntities;
using Character.Projectiles;
using Character.Vehicles;
using Chunks;
using Chunks.MarchingCubes;
using Common.Meshes;
using Common.UserInput;
using OpenTK.Mathematics;
using Physics.Collisions;
using Physics.Collisions.Bepu;
using Physics.TypingUtils;
using Player;


namespace Hyper;

// TODO fix this class.
internal class Scene : IInputSubscriber
{
    public readonly List<Chunk> Chunks;

    public readonly List<LightSource> LightSources;

    public readonly List<Projectile> Projectiles;

    public readonly List<Humanoid> Bots;

    public readonly SimpleCar SimpleCar; // TODO change it to a list of vehicles.

    public readonly Player.Player Player;

    public Camera Camera { get; set; }

    public readonly float Scale = 0.1f;

    private readonly CharacterControllers _characterControllers;

    public readonly SimulationManager<NarrowPhaseCallbacks, PoseIntegratorCallbacks> SimulationManager;

    public readonly CollidableProperty<SimulationProperties> Properties;

    public readonly Stopwatch Stopwatch = Stopwatch.StartNew();

    public Scene(float aspectRatio)
    {
        var scalarFieldGenerator = new ScalarFieldGenerator(1);
        ChunkFactory chunkFactory = new ChunkFactory(scalarFieldGenerator);
        int chunksPerSide = 2;

        Chunks = GetChunks(chunksPerSide, chunkFactory);
        LightSources = GetLightSources(chunksPerSide, scalarFieldGenerator.AvgElevation);
        Projectiles = new List<Projectile>();

        var bufferPool = new BufferPool();
        Properties = new CollidableProperty<SimulationProperties>();
        _characterControllers = new CharacterControllers(bufferPool);

        SimulationManager = new SimulationManager<NarrowPhaseCallbacks, PoseIntegratorCallbacks>(new NarrowPhaseCallbacks(_characterControllers, Properties),
            new PoseIntegratorCallbacks(new System.Numerics.Vector3(0, -10, 0)),
            new SolveDescription(6, 1), bufferPool);

        var characterInitialPosition = new Vector3(0, scalarFieldGenerator.AvgElevation + 8, 15);
        Player = new Player.Player(CreatePhysicalHumanoid(characterInitialPosition));

        int botsCount = 3;
        Bots = Enumerable.Range(0, botsCount) // initialize them however you like
            .Select(i => new Vector3(i * 4 - botsCount * 2, scalarFieldGenerator.AvgElevation + 5, i * 4 - botsCount * 2))
            .Select(pos => new Humanoid(CreatePhysicalHumanoid(pos)))
            .ToList();

        var carInitialPosition = new Vector3(5, scalarFieldGenerator.AvgElevation + 5, 12);
        SimpleCar = SimpleCar.CreateStandardCar(SimulationManager.Simulation, SimulationManager.BufferPool, Properties, Conversions.ToNumericsVector(carInitialPosition));

        Camera = GetCamera(aspectRatio, scalarFieldGenerator.AvgElevation);

        foreach (var chunk in Chunks)
        {
            chunk.CreateCollisionSurface(SimulationManager.Simulation, SimulationManager.BufferPool);
        }

        RegisterCallbacks();
    }

    private List<LightSource> GetLightSources(int chunksPerSide, float elevation)
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

                lightSources.Add(new LightSource(CubeMesh.Vertices, new Vector3(offset * x, elevation + 10f, offset * y), new Vector3(1, 1, 1)));
            }
        }

        return lightSources;
    }

    private static List<Chunk> GetChunks(int chunksPerSide, ChunkFactory generator)
    {
        return MakeSquare(chunksPerSide, generator);
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

    private Camera GetCamera(float aspectRatio, float elevation)
    {
        var camera = new Camera(aspectRatio, 0.01f, 100f, Scale)
        {
            ReferencePointPosition = (5f + elevation) * Vector3.UnitY
        };

        return camera;
    }

    private PhysicalCharacter CreatePhysicalHumanoid(Vector3 initialPosition)
        => new(_characterControllers, Properties, Conversions.ToNumericsVector(initialPosition),
            minimumSpeculativeMargin: 0.1f, mass: 1, maximumHorizontalForce: 20, maximumVerticalGlueForce: 100, jumpVelocity: 6, speed: 4,
            maximumSlope: MathF.PI * 0.4f);

    public void RegisterCallbacks()
    {
        Context context = Context.Instance;

        context.RegisterUpdateFrameCallback((e) =>
        {
            SimulationManager.Simulation.Timestep((float)e.Time, SimulationManager.ThreadDispatcher);
        });
    }
}
