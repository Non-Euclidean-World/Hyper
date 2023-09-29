namespace Hyper.PlayerData.InventorySystem.Items;

internal abstract class Item
{
    // Set according to what we have in the sprite sheet.
    public abstract string Id { get; }

    public abstract bool IsStackable { get; }

    public virtual void Use(Scene scene)
    {
        Console.WriteLine($"Used {Id}");
    }

    public virtual void SecondaryUse(Scene scene)
    {
        Console.WriteLine($"Used {Id} secondary");
    }
    
    public virtual void Use(Scene scene, float time)
    {
        Console.WriteLine($"Used {Id}");
    }

    public virtual void SecondaryUse(Scene scene, float time)
    {
        Console.WriteLine($"Used {Id} secondary");
    }
}