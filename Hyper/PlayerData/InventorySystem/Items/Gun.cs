using BepuPhysics;
using Character.Projectiles;

namespace Hyper.PlayerData.InventorySystem.Items;

internal class Gun : Item
{
    public override string Id => "gun";

    public override bool IsStackable => false;

    public override void Use(Scene scene)
    {
        if (!scene.Player.Inventory.TryRemoveItem("bullet")) return;
        CreateProjectile(scene);
    }

    private static void CreateProjectile(Scene scene)
    {
        var q = Helpers.CreateQuaternionFromTwoVectors(System.Numerics.Vector3.UnitX, scene.Player.RayDirection);
        var projectile = Projectile.CreateStandardProjectile(scene.SimulationManager.Simulation,
            scene.SimulationManager.Properties,
            new RigidPose(scene.Player.RayOrigin, q),
            scene.Player.RayDirection * 15,
            new ProjectileMesh(0.4f, 0.1f, 0.1f),
            lifeTime: 5,
            scene.Player.CurrentSphereId); // let's throw some refrigerators
        scene.Projectiles.Add(projectile);
        scene.SimulationMembers.Add(projectile);
    }
}