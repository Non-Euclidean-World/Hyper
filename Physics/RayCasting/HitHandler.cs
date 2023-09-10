// Copyright The Authors of bepuphysics2

using System.Numerics;
using System.Runtime.CompilerServices;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Trees;
using BepuUtilities.Memory;

namespace Physics.RayCasting;
public struct HitHandler : IRayHitHandler
{
    public Buffer<RayHit> Hits;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AllowTest(CollidableReference collidable)
    {
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AllowTest(CollidableReference collidable, int childIndex)
    {
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnRayHit(in RayData ray, ref float maximumT, float t, in Vector3 normal, CollidableReference collidable, int childIndex)
    {
        maximumT = t;
        ref var hit = ref Hits[ray.Id];
        if (t < hit.T)
        {
            hit.Normal = normal;
            hit.T = t;
            hit.Collidable = collidable;
            hit.Hit = true;
        }
    }
}
