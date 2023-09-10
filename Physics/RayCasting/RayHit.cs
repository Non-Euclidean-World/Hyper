// Copyright The Authors of bepuphysics2

using System.Numerics;
using BepuPhysics.Collidables;

namespace Physics.RayCasting;
public struct RayHit
{
    public Vector3 Normal;
    public float T;
    public CollidableReference Collidable;
    public bool Hit;
}