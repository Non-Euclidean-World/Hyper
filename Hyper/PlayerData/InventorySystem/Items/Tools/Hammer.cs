namespace Hyper.PlayerData.InventorySystem.Items.Tools;

public class Hammer : Tool
{
    public override string ID => "hammer";
    
    public override void Use()
    {
        Console.WriteLine("Digging...");
    }
}