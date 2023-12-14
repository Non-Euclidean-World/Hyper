using Chunks;
using Chunks.ChunkManagement.ChunkWorkers;
using OpenTK.Mathematics;
using ModificationFunc = System.Action<OpenTK.Mathematics.Vector3, float, float, int>;

namespace Hyper.PlayerData.InventorySystem.Items.Pickaxes;

internal abstract class Pickaxe : Item
{
    public override string Id => "pickaxe";

    public override bool IsStackable => false;

    protected virtual float BrushWeight => 3;

    public int Radius = 5;

    private float _mineTime = 0;

    private float _buildTime = 0;

    private static readonly Func<Chunk, ModificationFunc> Mining
        = (Chunk chunk) => (ModificationFunc)Delegate.CreateDelegate(typeof(ModificationFunc), chunk, typeof(Chunk).GetMethod("Mine")!);

    private static readonly Func<Chunk, ModificationFunc> Building
        = (Chunk chunk) => (ModificationFunc)Delegate.CreateDelegate(typeof(ModificationFunc), chunk, typeof(Chunk).GetMethod("Build")!);

    public override void Use(Scene scene, IChunkWorker chunkWorker, float time)
    {
        ModifyTerrain(scene, chunkWorker, time, ModificationType.Mine, ref _mineTime);
    }

    public override void SecondaryUse(Scene scene, IChunkWorker chunkWorker, float time)
    {
        ModifyTerrain(scene, chunkWorker, time, ModificationType.Build, ref _buildTime);
    }

    public override void Up()
    {
        Radius = Math.Min(Radius + 1, 10);
    }

    public override void Down()
    {
        Radius = Math.Max(Radius - 1, 1);
    }

    private void ModifyTerrain(Scene scene, IChunkWorker chunkWorker, float time, ModificationType modificationType, ref float modificationTime)
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

            ModificationArgs modificationArgs = new ModificationArgs();

            foreach (var chunk in chunkWorker.Chunks)
            {
                if (chunk.DistanceFromChunk(location) < Radius)
                {
                    modificationArgs.ModificationType = modificationType;
                    modificationArgs.Location = location;
                    modificationArgs.Time = time + modificationTime;
                    modificationArgs.BrushWeight = BrushWeight;
                    modificationArgs.Radius = Radius;
                    modificationArgs.Chunk = chunk;
                    chunkWorker.EnqueueModification(modificationArgs);
                }

                if (otherSphereLocation == null)
                    continue;
                if (chunk.DistanceFromChunk(otherSphereLocation.Value) < Radius)
                {
                    modificationArgs.ModificationType = modificationType;
                    modificationArgs.Location = location;
                    modificationArgs.Time = time + modificationTime;
                    modificationArgs.BrushWeight = BrushWeight;
                    modificationArgs.Radius = Radius;
                    modificationArgs.Chunk = chunk;
                    chunkWorker.EnqueueModification(modificationArgs);
                }
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