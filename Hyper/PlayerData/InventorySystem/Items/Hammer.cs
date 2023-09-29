namespace Hyper.PlayerData.InventorySystem.Items;

internal class Hammer : Item
{
    public override string Id => "hammer";

    public override bool IsStackable { get; } = false;
    
    private float _mineTime = 0;
    
    private float _buildTime = 0;

    public override void Use(Scene scene, float time)
    {
        foreach (var chunk in scene.ChunkWorker.Chunks)
        {
            var location =
                scene.Player.GetRayEndpoint(in scene.SimulationManager.RayCastingResults[scene.Player.RayId]);
            if (!chunk.IsInside(location)) continue;
            if (!scene.ChunkWorker.IsOnUpdateQueue(chunk))
            {
                chunk.Mine(location, time + _mineTime);
                scene.ChunkWorker.EnqueueUpdatingChunk(chunk);
                _mineTime = 0;
            }
            else _mineTime += time;
            return;
        }
    }

    public override void SecondaryUse(Scene scene, float time)
    {
        foreach (var chunk in scene.ChunkWorker.Chunks)
        {
            var location =
                scene.Player.GetRayEndpoint(in scene.SimulationManager.RayCastingResults[scene.Player.RayId]);
            if (!chunk.IsInside(location)) continue;
            if (!scene.ChunkWorker.IsOnUpdateQueue(chunk))
            {
                chunk.Build(location, time + _buildTime);
                scene.ChunkWorker.EnqueueUpdatingChunk(chunk);
                _buildTime = 0;
            }
            else _buildTime += time;
            return;
        }
    }
}