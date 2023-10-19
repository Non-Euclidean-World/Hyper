using Chunks.ChunkManagement.ChunkWorkers;

namespace Hyper.PlayerData.InventorySystem.Items;

internal abstract class Item
{
    // Set according to what we have in the sprite sheet.
    public abstract string Id { get; }

    public abstract bool IsStackable { get; }

    public virtual void Use(Scene scene) { }

    public virtual void SecondaryUse(Scene scene) { }

    public virtual void Use(Scene scene, IChunkWorker chunkWorker, float time) { }

    public virtual void SecondaryUse(Scene scene, IChunkWorker chunkWorker, float time) { }

    public virtual void Up() { }
    
    public virtual void Down() { }
}