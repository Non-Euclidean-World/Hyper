namespace Hyper.PlayerData.InventorySystem.Items.Tools;

public class Tool : Item
{
    public sealed override bool IsStackable { get; } = false;
}