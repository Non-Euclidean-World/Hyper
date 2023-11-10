using Common.Meshes;
using OpenTK.Mathematics;
using Physics.TypingUtils;

namespace Hyper.PlayerData.InventorySystem.Items;
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

    private static void CreateLamp(Scene scene)
    {
        scene.LightSources.Add(new LightSource(CubeMesh.Vertices,
                    position: new Vector3(Conversions.ToOpenTKVector(scene.Player.RayOrigin)),
                    color: new Vector3(1, 1, 1),
                    ambient: new Vector3(0.05f, 0.05f, 0.05f),
                    diffuse: new Vector3(0.8f, 0.8f, 0.8f),
                    specular: new Vector3(1f, 1f, 1f),
                    constant: 1f,
                    linear: 0.35f,
                    quadratic: 0.44f));
    }
}
