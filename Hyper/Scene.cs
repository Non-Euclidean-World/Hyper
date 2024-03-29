﻿using System.Diagnostics;
using BepuPhysics;
using Character.LightSources;
using Character.Projectiles;
using Character.Vehicles;
using Chunks;
using Common.Meshes;
using Common.UserInput;
using Hyper.GameEntities;
using Hyper.PlayerData;
using OpenTK.Mathematics;
using Physics;
using Physics.Collisions;
using Physics.TypingUtils;

namespace Hyper;

internal class Scene : IInputSubscriber
{
    public readonly List<Chunk> Chunks = new();

    public readonly List<Lamp> LightSources = new();

    public readonly List<FlashLight> FlashLights = new();

    public readonly List<Projectile> Projectiles = new();

    public readonly List<AstronautBot> Bots = new();

    public readonly List<FourWheeledCar> FreeCars = new();

    public FourWheeledCar? PlayersCar { get; private set; }

    public Player Player { get; private set; } = null!;

    public readonly Camera Camera;

    public readonly SimulationMembers SimulationMembers = new();

    public readonly SimulationManager<PoseIntegratorCallbacks> SimulationManager;

    public Vector3i[]? SphereCenters;

    public readonly float GlobalScale;

    private Context _context;

    public Scene(Camera camera, float globalScale, Context context)
    {
        SimulationManager = new SimulationManager<PoseIntegratorCallbacks>(
            new PoseIntegratorCallbacks(new System.Numerics.Vector3(0, -10, 0)),
            new SolveDescription(6, 1),
            timeStepSeconds: 1 / 60f);

        _context = context;


        Camera = camera;
        GlobalScale = globalScale;

        RegisterCallbacks(context);
        SimulationManager.RegisterUpdateAction(UpdateAction);
    }

    public void SpawnPlayer(Vector3 location)
    {
        Player = new Player(Humanoid.CreatePhysicalCharacter(location, SimulationManager), _context);
        FlashLights.Add(Player.FlashLight);
        SimulationMembers.Add(Player);
        SimulationManager.RegisterContactCallback(Player.BodyHandle, contactInfo => Player.ContactCallback(contactInfo, SimulationMembers));
    }

    /// <summary>
    /// Makes the player enter the closest car.
    /// </summary>
    /// <param name="testOnly">If set to true, the player won't actually enter the car</param>
    /// <returns>True if the player could enter the car, false otherwise</returns>
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
                    Camera.FirstPerson = false;
                }
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Flips the car closest to the player.
    /// </summary>
    /// <param name="testOnly">If set to true, the car won't actually be flipped</param>
    /// <returns>True if the car could be flipped, false otherwise</returns>
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
    /// Picks the lamp that is closest to the player.
    /// </summary>
    /// <param name="testOnly">If true the lamp won't be picked even if it could be</param>
    /// <returns>True if a lamp was (or could be) picked. False otherwise.</returns>
    public bool TryPickLamp(bool testOnly = false)
    {
        const float lampPickRadius = 10f;

        Lamp? closestLamp = LightSources.MinBy(lamp => System.Numerics.Vector3.Distance(Conversions.ToNumericsVector(lamp.Position), Player.PhysicalCharacter.Pose.Position));
        if (closestLamp == null)
            return false;

        if (System.Numerics.Vector3.Distance(Conversions.ToNumericsVector(closestLamp.Position), Player.PhysicalCharacter.Pose.Position) > lampPickRadius)
            return false;

        if (!testOnly)
        {
            LightSources.Remove(closestLamp);
            Player.Inventory.AddItem(new PlayerData.InventorySystem.Items.Lamp());
        }

        return true;
    }

    /// <summary>
    /// Makes the player leave the car.
    /// </summary>
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
        Debug.Assert(Player.BodyHandle == Player.BodyHandles[0]);

        SimulationMembers.Add(Player);
        FlashLights.Add(Player.FlashLight);
    }

    public void RegisterCallbacks(Context context)
    {
        context.RegisterUpdateFrameCallback((_) =>
        {
            SimulationManager.Update();
        });
    }

    public void Dispose()
    {
        foreach (var chunk in Chunks)
            chunk.Dispose(SimulationManager.Simulation, SimulationManager.BufferPool);

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

    private void UpdateAction()
    {
        SimulationManager.FlushContactEvents();
        SimulationManager.ResetRayCastingResult(Player, Player.RayId);
        SimulationManager.RayCast(Player, Player.RayId);
    }
}
