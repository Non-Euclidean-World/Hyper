namespace Hyper.PlayerData.InventorySystem.Items;

public abstract class Item
{
    public virtual bool IsStackable { get; }

    public virtual void Use()
    {
        Console.WriteLine($"Used {this.GetType().Name}");
    }
}