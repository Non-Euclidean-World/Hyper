using Chunks.ChunkManagement.ChunkWorkers;

namespace Hyper.PlayerData.InventorySystem.Items;

internal abstract class Item : IEquatable<Item>
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

    public bool Equals(Item? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return other.Id == Id;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Item);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}