using System.Diagnostics;
using Chunks.ChunkManagement.ChunkWorkers;
using OpenTK.Mathematics;

namespace Hyper.PlayerData.InventorySystem.Items.Pickaxes;

internal abstract class Pickaxe : Item
{
    public override string Id => "pickaxe";

    public override bool IsStackable => false;

    protected virtual float BrushWeight => 3;

    public int Radius = 5;

    private float _mineTime = 0;

    private float _buildTime = 0;

    private double _timeAccumulator = 0;

    private readonly Stopwatch _stopwatch = new();

    private const float TimeStepSeconds = 1 / 30f;

    private double _prev = 0;

    public override void Use(Scene scene, IChunkWorker chunkWorker, float time)
    {
        ModifyTerrainTimeStepped(scene, chunkWorker, time, ModificationType.Mine, ref _mineTime);
    }

    public override void SecondaryUse(Scene scene, IChunkWorker chunkWorker, float time)
    {
        ModifyTerrainTimeStepped(scene, chunkWorker, time, ModificationType.Build, ref _buildTime);
    }

    public void StartUsing()
    {
        _prev = 0;
        _stopwatch.Restart();
    }

    public override void Up()
    {
        Radius = Math.Min(Radius + 1, 10);
    }

    public override void Down()
    {
        Radius = Math.Max(Radius - 1, 1);
    }

    private void ModifyTerrainTimeStepped(Scene scene, IChunkWorker chunkWorker, float time, ModificationType modificationType, ref float modificationTime)
    {
        double now = _stopwatch.ElapsedTicks;
        double dt = (now - _prev) / Stopwatch.Frequency;
        _timeAccumulator += dt;
        _prev = now;
        while (_timeAccumulator >= TimeStepSeconds)
        {
            ModifyTerrain(scene, chunkWorker, TimeStepSeconds, modificationType, ref modificationTime);

            _timeAccumulator -= TimeStepSeconds;
        }
    }

    private readonly List<ModificationArgs> _buffer = new();

    private void ModifyTerrain(Scene scene, IChunkWorker chunkWorker, float time, ModificationType modificationType, ref float modificationTime)
    {
        bool zeroTime = false;
        if (!chunkWorker.IsProcessingBatch)
        {
            var location = scene.Player.GetRayEndpoint(in scene.SimulationManager.RayCastingResults[scene.Player.RayId]);
            Vector3? otherSphereLocation = null;
            if (scene.Camera.Curve > 0)
            {
                otherSphereLocation = GetOtherSphereLocation(scene.Camera.Sphere, location, scene);
            }

            ModificationArgs modificationArgs = new ModificationArgs
            {
                ModificationType = modificationType,
                Location = location,
                Time = time + modificationTime,
                BrushWeight = BrushWeight,
                Radius = Radius
            };

            _buffer.Clear();
            foreach (var chunk in chunkWorker.Chunks)
            {
                modificationArgs.Chunk = chunk;
                if (chunk.DistanceFromChunk(location) < Radius)
                {
                    modificationArgs.Location = location;
                    _buffer.Add(modificationArgs);
                }

                if (otherSphereLocation == null)
                    continue;
                if (chunk.DistanceFromChunk(otherSphereLocation.Value) < Radius)
                {
                    modificationArgs.Location = otherSphereLocation.Value;
                    _buffer.Add(modificationArgs);
                }
            }

            chunkWorker.IsProcessingBatch = true;

            for (int i = 0; i < _buffer.Count; i++)
            {
                var args = _buffer[i];
                args.BatchSize = _buffer.Count;
                chunkWorker.EnqueueModification(args);
            }

            zeroTime = true;
        }

        if (zeroTime) modificationTime = 0;
        else modificationTime += time;
    }

    private static Vector3 GetOtherSphereLocation(int currentSphere, Vector3 location, Scene scene)
    {
        int otherSphere = 1 - currentSphere;

        float distanceFromCenter = Vector3.Distance(Flatten(location), scene.SphereCenters![currentSphere]);
        float sphereRadius = MathF.PI / 2 / scene.GlobalScale;
        var otherSphereMineLocation
            = scene.SphereCenters![otherSphere]
            + FlipXZ(Vector3.Normalize(Flatten(location) - scene.SphereCenters![currentSphere]) * (2 * sphereRadius - distanceFromCenter))
            + location.Y * Vector3.UnitY;

        return otherSphereMineLocation;
    }

    private static Vector3 FlipXZ(Vector3 v) => new(-v.X, v.Y, -v.Z);

    private static Vector3 Flatten(Vector3 v) => new(v.X, 0, v.Z);
}