namespace Hyper.PlayerData.InventorySystem.Items;

public class Sword : Item
{
    public override string ID => "sword";
    
    public override void Use()
    {
        Console.WriteLine("Cutting...");
    }
}