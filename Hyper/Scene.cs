﻿using BepuPhysics;
using Character.GameEntities;
using Character.Projectiles;
using Character.Vehicles;
using Chunks;
using Common.Meshes;
using Common.UserInput;
using Hyper.PlayerData;
using OpenTK.Mathematics;
using Physics;
using Physics.Collisions;

namespace Hyper;

internal class Scene : IInputSubscriber
{
    public readonly List<Chunk> Chunks;

    public readonly List<LightSource> LightSources;

    public readonly List<Projectile> Projectiles;

    public readonly List<Humanoid> Bots = new();

    public readonly List<FourWheeledCar> FreeCars = new();

    public FourWheeledCar? PlayersCar { get; private set; }

    public Player Player { get; private set; }

    public readonly Camera Camera;

    public readonly SimulationMembers SimulationMembers;

    public readonly SimulationManager<PoseIntegratorCallbacks> SimulationManager;

    public Scene(Camera camera, float elevation, Context context)
    {
        int chunksPerSide = 2;

        LightSources = GetLightSources(chunksPerSide, elevation);
        Projectiles = new List<Projectile>();

        SimulationMembers = new SimulationMembers();
        SimulationManager = new SimulationManager<PoseIntegratorCallbacks>(
            new PoseIntegratorCallbacks(new System.Numerics.Vector3(0, -10, 0)),
            new SolveDescription(6, 1));

        Player = new Player(Humanoid.CreatePhysicalCharacter(new Vector3(0, elevation + 5, 0), SimulationManager), context);
        SimulationMembers.Add(Player);
        SimulationManager.RegisterContactCallback(Player.BodyHandle, contactInfo => Player.ContactCallback(contactInfo, SimulationMembers));

        Camera = camera;

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

                lightSources.Add(new LightSource(CubeMesh.Vertices,
                    position: new Vector3(offset * x, elevation + 10f, offset * y),
                    color: new Vector3(1, 1, 1),
                    ambient: new Vector3(0.05f, 0.05f, 0.05f),
                    diffuse: new Vector3(0.8f, 0.8f, 0.8f),
                    specular: new Vector3(1f, 1f, 1f),
                    constant: 1f,
                    linear: 0.35f,
                    quadratic: 0.44f));
            }
        }

        return lightSources;
    }

    public bool TryEnterClosestCar(bool testOnly = false)
    {
        const float carEnterRadius = 10f;
        foreach (var car in FreeCars)
        {
            if (System.Numerics.Vector3.Distance(car.CarBodyPose.Position, Player.PhysicalCharacter.Pose.Position) <= carEnterRadius)
            {
                if (!testOnly)
                {
                    PlayersCar = car;
                    FreeCars.Remove(car);
                    Player.Hide();
                    SimulationMembers.Remove(Player);
                }
                return true;
            }
        }

        return false;
    }

    public bool TryFlipClosestCar(bool testOnly = false)
    {
        const float carEnterRadius = 10f;
        for (int i = 0; i < FreeCars.Count; i++)
        {
            var car = FreeCars[i];
            if (System.Numerics.Vector3.Distance(car.CarBodyPose.Position, Player.PhysicalCharacter.Pose.Position) <= carEnterRadius)
            {
                if (!testOnly)
                {
                    FreeCars[i] = new SpaceMustang(SimpleCar.CreateStandardCar(SimulationManager.Simulation, SimulationManager.BufferPool, SimulationManager.Properties,
                        car.CarBodyPose.Position + System.Numerics.Vector3.UnitY), car.CurrentSphereId);
                    SimulationManager.Simulation.Awakener.AwakenBody(FreeCars[i].BodyHandle);
                    SimulationMembers.Add(FreeCars[i]);
                    SimulationMembers.Remove(car);
                    car.Dispose();
                }
                return true;
            }

        }

        return false;
    }

    public void LeaveCar()
    {
        if (PlayersCar == null)
            return;

        var position = PlayersCar.CarBodyPose.Position;
        FreeCars.Add(PlayersCar);
        Player.CurrentSphereId = PlayersCar.CurrentSphereId;
        PlayersCar = null;

        Player.Show(Humanoid.CreatePhysicalCharacter(new Vector3(position.X, position.Y + 5, position.Z), SimulationManager));
        SimulationMembers.Add(Player);
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

        foreach (var car in FreeCars)
            car.Dispose();

        PlayersCar?.Dispose();

        Player.Dispose();

        SimulationManager.Dispose();
    }
}
