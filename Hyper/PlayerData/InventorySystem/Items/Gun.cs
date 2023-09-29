﻿using BepuPhysics;
using Character.Projectiles;
using Physics.TypingUtils;

namespace Hyper.PlayerData.InventorySystem.Items;

internal class Gun : Item
{
    public override string ID => "gun";
    
    public override bool IsStackable => false;
    
    public override void Use(Scene scene)
    {
        if (!scene.Player.Inventory.TryRemoveItem("bullet")) return;
        CreateProjectile(scene);
    }
    
    private void CreateProjectile(Scene scene)
    {
        var q = Helpers.CreateQuaternionFromTwoVectors(System.Numerics.Vector3.UnitX, Conversions.ToNumericsVector(scene.Camera.Front));
        var projectile = Projectile.CreateStandardProjectile(scene.SimulationManager.Simulation,
            scene.SimulationManager.Properties,
            new RigidPose(scene.Player.RayOrigin, q),
            Conversions.ToNumericsVector(scene.Camera.Front) * 15,
            new ProjectileMesh(2, 0.5f, 0.5f), lifeTime: 5); // let's throw some refrigerators
        scene.Projectiles.Add(projectile);
        scene.SimulationMembers.Add(projectile.BodyHandle, projectile);
    }
}