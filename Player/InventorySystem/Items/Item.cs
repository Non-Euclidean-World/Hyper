namespace Player.InventorySystem.Items;

public abstract class Item
{
    // Set according to what we have in the sprite sheet.
    public abstract string ID { get; }

    public abstract bool IsStackable { get; }

    public virtual void Use()
    {
        Console.WriteLine($"Used {ID}");
    }
}