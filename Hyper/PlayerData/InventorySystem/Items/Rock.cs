namespace Hyper.PlayerData.InventorySystem.Items;

public class Rock : Item
{
    public override string ID => "rock";
    
    public override bool IsStackable => true;
    
    public override void Use()
    {
        Console.WriteLine("Rocking...");
    }
}