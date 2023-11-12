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
using Physics.TypingUtils;

namespace Hyper;

internal class Scene : IInputSubscriber
{
    public readonly List<Chunk> Chunks;

    public readonly List<Common.Meshes.Lamp> LightSources = new();

    public readonly List<FlashLight> FlashLights = new();

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
        Projectiles = new List<Projectile>();

        SimulationMembers = new SimulationMembers();
        SimulationManager = new SimulationManager<PoseIntegratorCallbacks>(
            new PoseIntegratorCallbacks(new System.Numerics.Vector3(0, -10, 0)),
            new SolveDescription(6, 1));

        Player = new Player(Humanoid.CreatePhysicalCharacter(new Vector3(0, elevation + 5, 0), SimulationManager), context);
        FlashLights.Add(Player.FlashLight);
        SimulationMembers.Add(Player);
        SimulationManager.RegisterContactCallback(Player.BodyHandle, contactInfo => Player.ContactCallback(contactInfo, SimulationMembers));

        Camera = camera;

        Chunks = new List<Chunk>();

        RegisterCallbacks(context);
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
                    FlashLights.Remove(Player.FlashLight);
                    FlashLights.AddRange(PlayersCar.Lights);
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
                    FlashLights.RemoveAll(car.Lights.Contains);
                    car.Dispose();
                }
                return true;
            }

        }

        return false;
    }

    /// <summary>
    /// Picks a lamp if it's close enough to the player.
    /// </summary>
    /// <param name="testOnly">If true the lamp won't be picked even if it could be</param>
    /// <returns>True if a lamp was (or could be) picked. False otherwise.</returns>
    public bool TryPickLamp(bool testOnly = false)
    {
        const float lampPickRadius = 10f;
        for (int i = 0; i < LightSources.Count; i++)
        {
            var lamp = LightSources[i];
            if (System.Numerics.Vector3.Distance(Conversions.ToNumericsVector(lamp.Position), Player.PhysicalCharacter.Pose.Position) <= lampPickRadius)
            {
                if (!testOnly)
                {
                    LightSources.RemoveAt(i);
                    Player.Inventory.AddItem(new PlayerData.InventorySystem.Items.Lamp());
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
        FlashLights.RemoveAll(PlayersCar.Lights.Contains);
        PlayersCar = null;

        Player.Show(Humanoid.CreatePhysicalCharacter(new Vector3(position.X, position.Y + 5, position.Z), SimulationManager));
        SimulationMembers.Add(Player);
        FlashLights.Add(Player.FlashLight);
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
