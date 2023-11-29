using OpenTK.Mathematics;
using Physics.TypingUtils;

namespace Hyper.PlayerData.InventorySystem.Items;

/// <summary>
/// Item that represents a lamp in the inventory.
/// </summary>
internal class Lamp : Item
{
    public override string Id => "lamp";

    public override bool IsStackable => true;

    public override void Use(Scene scene)
    {
        if (!scene.Player.Inventory.TryRemoveItem("lamp"))
            return;
        CreateLamp(scene);
    }

    public override void SecondaryUse(Scene scene)
    {
        scene.TryPickLamp();
    }

    private static void CreateLamp(Scene scene)
    {
        scene.LightSources.Add(Common.Meshes.Lamp.CreateStandardLamp(Conversions.ToOpenTKVector(scene.Player.RayOrigin), Vector3.One));
    }
}
