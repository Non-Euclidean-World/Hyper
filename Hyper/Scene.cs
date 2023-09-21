﻿using System.Diagnostics;
using BepuPhysics;
using Character.GameEntities;
using Character.Projectiles;
using Character.Vehicles;
using Chunks;
using Common.Meshes;
using Common.UserInput;
using OpenTK.Mathematics;
using Physics.Collisions;
using Physics.TypingUtils;
using Player;


namespace Hyper;

internal class Scene : IInputSubscriber
{
    public readonly List<Chunk> Chunks = new();

    public readonly List<LightSource> LightSources;

    public readonly List<Projectile> Projectiles;

    public readonly List<Humanoid> Bots;

    public readonly List<SimpleCar> Cars;

    public readonly Player.Player Player;

    public readonly Camera Camera;

    public readonly Dictionary<BodyHandle, ISimulationMember> SimulationMembers;

    public readonly SimulationManager<PoseIntegratorCallbacks> SimulationManager;

    public readonly float Scale = 0.1f;

    public readonly Stopwatch Stopwatch = Stopwatch.StartNew();

    public Scene(float aspectRatio, float elevation)
    {
        int chunksPerSide = 2;

        LightSources = GetLightSources(chunksPerSide, elevation);
        Projectiles = new List<Projectile>();

        SimulationMembers = new Dictionary<BodyHandle, ISimulationMember>();
        SimulationManager = new SimulationManager<PoseIntegratorCallbacks>(
            new PoseIntegratorCallbacks(new System.Numerics.Vector3(0, -10, 0)),
            new SolveDescription(6, 1));

        int botsCount = 3;
        Bots = Enumerable.Range(0, botsCount) // initialize them however you like
            .Select(i => new Vector3(i * 4 - botsCount * 2, elevation + 5, i * 4 - botsCount * 2))
            .Select(pos =>
            {
                var humanoid = new Humanoid(CreatePhysicalHumanoid(pos));
                SimulationMembers.Add(humanoid.BodyHandle, humanoid);
                SimulationManager.RegisterContactCallback(humanoid.BodyHandle, contactInfo => humanoid.ContactCallback(contactInfo, SimulationMembers));
                return humanoid;
            })
            .ToList();
        
        Player = new Player.Player(CreatePhysicalHumanoid(new Vector3(0, elevation + 5, 0)));

        var carInitialPosition = new Vector3(5, elevation + 5, 12);
        Cars = new List<SimpleCar>()
        {
            SimpleCar.CreateStandardCar(SimulationManager.Simulation, SimulationManager.BufferPool, SimulationManager.Properties,
                Conversions.ToNumericsVector(carInitialPosition))
        };

        Camera = GetCamera(aspectRatio, elevation);

        RegisterCallbacks();
    }

    private static List<LightSource> GetLightSources(int chunksPerSide, float elevation)
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

    private Camera GetCamera(float aspectRatio, float elevation)
    {
        var camera = new Camera(aspectRatio, 0.01f, 100f, Scale)
        {
            ReferencePointPosition = (5f + elevation) * Vector3.UnitY
        };

        return camera;
    }

    public PhysicalCharacter CreatePhysicalHumanoid(Vector3 initialPosition)
        => new(SimulationManager.CharacterControllers, SimulationManager.Properties, Conversions.ToNumericsVector(initialPosition),
            minimumSpeculativeMargin: 0.1f, mass: 1, maximumHorizontalForce: 20, maximumVerticalGlueForce: 100, jumpVelocity: 6, speed: 4,
            maximumSlope: MathF.PI * 0.4f);

    public void RegisterCallbacks()
    {
        Context context = Context.Instance;

        context.RegisterUpdateFrameCallback((e) =>
        {
            SimulationManager.Timestep((float)e.Time);
            SimulationManager.FlushContactEvents();
            SimulationManager.ResetRayCastingResult(Player, Player.RayId);
            SimulationManager.RayCast(Player, Player.RayId);
        });
    }

    public void Dispose()
    {
        foreach (var chunk in Chunks)
            chunk.Dispose(SimulationManager.Simulation, SimulationManager.BufferPool);
        Chunks.Clear();
        
        foreach (var lightSource in LightSources)
            lightSource.Dispose();
        LightSources.Clear();
        
        foreach (var projectile in Projectiles)
            projectile.Dispose(SimulationManager.Simulation, SimulationManager.BufferPool);
        Projectiles.Clear();
        
        foreach (var bot in Bots)
            bot.Dispose();
        Bots.Clear();
        
        Player.Dispose();
        
        SimulationMembers.Clear();
        
        SimulationManager.Dispose();
        
        Stopwatch.Stop();
    }
}
