namespace Player.InventorySystem.Items.Tools;

public class Hammer : Item
{
    public override string ID => "hammer";

    public override bool IsStackable { get; } = false;

    public override void Use()
    {
        Console.WriteLine("Digging...");
    }
}