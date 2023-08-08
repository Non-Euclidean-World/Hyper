namespace Hyper.PlayerData.InventorySystem.Items;

public abstract class Item
{
    // Set according to what we have in the sprite sheet.
    public virtual string ID { get; }
    
    public virtual bool IsStackable { get; }

    public virtual void Use()
    {
        Console.WriteLine($"Used {ID}");
    }
}