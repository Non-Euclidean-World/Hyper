﻿using BepuPhysics;
using Character.GameEntities;
using Character.Projectiles;
using Character.Vehicles;
using Chunks;
using Common;
using Common.Meshes;
using Common.UserInput;
using Hud;
using Hud.HUDElements;
using Hyper.PlayerData;
using Hyper.PlayerData.InventorySystem.InventoryRendering;
using OpenTK.Mathematics;
using Physics.Collisions;
using Physics.TypingUtils;


namespace Hyper;

internal class Scene : IInputSubscriber
{
    public readonly List<Chunk> Chunks;

    public readonly List<LightSource> LightSources;

    public readonly List<Projectile> Projectiles;

    public readonly List<Humanoid> Bots = new();

    public readonly List<SimpleCar> Cars;

    public readonly Player Player;

    public readonly Camera Camera;

    public readonly Dictionary<BodyHandle, ISimulationMember> SimulationMembers;

    public readonly SimulationManager<PoseIntegratorCallbacks> SimulationManager;

    public readonly IHudElement[] HudElements;

    public Scene(Camera camera, float elevation, Context context, IWindowHelper windowHelper)
    {
        int chunksPerSide = 2;

        LightSources = GetLightSources(chunksPerSide, elevation);
        Projectiles = new List<Projectile>();

        SimulationMembers = new Dictionary<BodyHandle, ISimulationMember>();
        SimulationManager = new SimulationManager<PoseIntegratorCallbacks>(
            new PoseIntegratorCallbacks(new System.Numerics.Vector3(0, -10, 0)),
            new SolveDescription(6, 1));

        Player = new Player(Humanoid.CreatePhysicalCharacter(new Vector3(0, elevation + 5, 0), SimulationManager), context);
        SimulationMembers.Add(Player.BodyHandle, Player);
        SimulationManager.RegisterContactCallback(Player.BodyHandle, contactInfo => Player.ContactCallback(contactInfo, SimulationMembers));

        var carInitialPosition = new Vector3(5, elevation + 5, 12);
        Cars = new List<SimpleCar>()
        {
            SimpleCar.CreateStandardCar(SimulationManager.Simulation, SimulationManager.BufferPool, SimulationManager.Properties,
                Conversions.ToNumericsVector(carInitialPosition))
        };

        Camera = camera;

        HudElements = new IHudElement[]
        {
            new Crosshair(),
            new FpsCounter(windowHelper),
            new InventoryHudManager(windowHelper, Player.Inventory, context),
        };

        Chunks = new List<Chunk>();
        RegisterCallbacks(context);
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

    public void RegisterCallbacks(Context context)
    {
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

        foreach (var lightSource in LightSources)
            lightSource.Dispose();

        foreach (var projectile in Projectiles)
            projectile.Dispose(SimulationManager.Simulation, SimulationManager.BufferPool);

        foreach (var bot in Bots)
            bot.Dispose();

        Player.Dispose();

        SimulationManager.Dispose();

        foreach (var element in HudElements)
        {
            element.Dispose();
        }
    }
}
