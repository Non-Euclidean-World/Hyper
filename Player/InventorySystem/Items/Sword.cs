namespace Player.InventorySystem.Items;

public class Sword : Item
{
    public override string ID => "sword";
    
    public override bool IsStackable { get; } = false;

    public override void Use()
    {
        Console.WriteLine("Cutting...");
    }
}