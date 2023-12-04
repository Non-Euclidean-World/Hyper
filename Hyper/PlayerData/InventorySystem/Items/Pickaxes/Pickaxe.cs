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

    public override void Use(Scene scene, IChunkWorker chunkWorker, float time)
    {
        bool zeroTime = false;
        if (!chunkWorker.IsUpdating)
        {
            var location = scene.Player.GetRayEndpoint(in scene.SimulationManager.RayCastingResults[scene.Player.RayId]);
            Vector3? otherSphereLocation = null;
            if (scene.Camera.Curve > 0)
            {
                otherSphereLocation = GetOtherSphereLocation(scene.Camera.Sphere, location, scene);
            }

            foreach (var chunk in chunkWorker.Chunks)
            {
                if (chunk.DistanceFromChunk(location) < Radius)
                {
                    chunk.Mine(location, time + _mineTime, BrushWeight, Radius);
                    chunkWorker.EnqueueUpdatingChunk(chunk);
                }

                if (otherSphereLocation == null)
                    continue;
                if (chunk.DistanceFromChunk(otherSphereLocation.Value) < Radius)
                {
                    chunk.Mine(otherSphereLocation.Value, time + _mineTime, BrushWeight, Radius);
                    chunkWorker.EnqueueUpdatingChunk(chunk);
                }
            }

            zeroTime = true;
        }

        if (zeroTime) _mineTime = 0;
        else _mineTime += time;
    }

    public override void SecondaryUse(Scene scene, IChunkWorker chunkWorker, float time)
    {
        bool zeroTime = false;
        if (!chunkWorker.IsUpdating)
        {
            var location = scene.Player.GetRayEndpoint(in scene.SimulationManager.RayCastingResults[scene.Player.RayId]);
            Vector3? otherSphereLocation = null;
            if (scene.Camera.Curve > 0)
            {
                otherSphereLocation = GetOtherSphereLocation(scene.Camera.Sphere, location, scene);
            }
            foreach (var chunk in chunkWorker.Chunks)
            {
                if (chunk.DistanceFromChunk(location) < Radius)
                {
                    chunk.Build(location, time + _buildTime, BrushWeight, Radius);
                    chunkWorker.EnqueueUpdatingChunk(chunk);
                }

                if (otherSphereLocation == null)
                    continue;
                if (chunk.DistanceFromChunk(otherSphereLocation.Value) < Radius)
                {
                    chunk.Build(otherSphereLocation.Value, time + _buildTime, BrushWeight, Radius);
                    chunkWorker.EnqueueUpdatingChunk(chunk);
                }
            }
            zeroTime = true;
        }

        if (zeroTime) _buildTime = 0;
        else _buildTime += time;
    }

    public override void Up()
    {
        Radius = Math.Min(Radius + 1, 10);
    }

    public override void Down()
    {
        Radius = Math.Max(Radius - 1, 1);
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