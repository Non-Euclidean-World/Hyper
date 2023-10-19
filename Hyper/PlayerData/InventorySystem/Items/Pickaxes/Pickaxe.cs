using Chunks.ChunkManagement.ChunkWorkers;

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
        foreach (var chunk in chunkWorker.Chunks)
        {
            var location =
                scene.Player.GetRayEndpoint(in scene.SimulationManager.RayCastingResults[scene.Player.RayId]);
            if (chunk.DistanceFromChunk(location) >= Radius) continue;
            if (chunkWorker.IsOnUpdateQueue(chunk)) continue;
            chunk.Mine(location, time + _mineTime, BrushWeight, Radius);
            chunkWorker.EnqueueUpdatingChunk(chunk);
            zeroTime = true;
        }

        if (zeroTime) _mineTime = 0;
        else _mineTime += time;
    }

    public override void SecondaryUse(Scene scene, IChunkWorker chunkWorker, float time)
    {
        bool zeroTime = false;
        foreach (var chunk in chunkWorker.Chunks)
        {
            var location =
                scene.Player.GetRayEndpoint(in scene.SimulationManager.RayCastingResults[scene.Player.RayId]);
            if (chunk.DistanceFromChunk(location) >= Radius) continue;
            if (chunkWorker.IsOnUpdateQueue(chunk)) continue;
            chunk.Build(location, time + _buildTime, BrushWeight, Radius);
            chunkWorker.EnqueueUpdatingChunk(chunk);
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
}