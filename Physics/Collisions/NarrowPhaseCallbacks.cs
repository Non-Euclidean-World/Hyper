﻿// Copyright The Authors of bepuphysics2

// changes: added characters handling from `CharacterNarrowPhaseCallbacks.cs`

using System.Runtime.CompilerServices;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;
using Physics.ContactCallbacks;

namespace Physics.Collisions;

public struct NarrowPhaseCallbacks : INarrowPhaseCallbacks
{
    public CollidableProperty<SimulationProperties> Properties;
    public CharacterControllers Characters;
    private ContactEvents _contactEvents;

    public void Initialize(Simulation simulation)
    {
        Properties.Initialize(simulation);
        Characters.Initialize(simulation);
        _contactEvents.Initialize(simulation);
    }

    public NarrowPhaseCallbacks(CharacterControllers characters, CollidableProperty<SimulationProperties> properties, ContactEvents contactEvents)
    {
        Characters = characters;
        Properties = properties;
        _contactEvents = contactEvents;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AllowContactGeneration(int workerIndex, CollidableReference a, CollidableReference b, ref float speculativeMargin)
    {
        //It's impossible for two statics to collide, and pairs are sorted such that bodies always come before statics.
        if (b.Mobility != CollidableMobility.Static)
        {
            return SubgroupCollisionFilter.AllowCollision(Properties[a.BodyHandle].Filter, Properties[b.BodyHandle].Filter);
        }
        return a.Mobility == CollidableMobility.Dynamic || b.Mobility == CollidableMobility.Dynamic;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AllowContactGeneration(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB)
    {
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe bool ConfigureContactManifold<TManifold>(int workerIndex, CollidablePair pair, ref TManifold manifold, out PairMaterialProperties pairMaterial) where TManifold : unmanaged, IContactManifold<TManifold>
    {
        pairMaterial.FrictionCoefficient = Properties[pair.A.BodyHandle].Friction;
        if (pair.B.Mobility != CollidableMobility.Static)
        {
            //If two bodies collide, just average the friction.
            pairMaterial.FrictionCoefficient = (pairMaterial.FrictionCoefficient + Properties[pair.B.BodyHandle].Friction) * 0.5f;
        }
        pairMaterial.MaximumRecoveryVelocity = 2f;
        pairMaterial.SpringSettings = new SpringSettings(30, 1);
        Characters.TryReportContacts(pair, ref manifold, workerIndex, ref pairMaterial);
        _contactEvents.HandleManifold(workerIndex, pair, ref manifold);
        return true;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe bool ConfigureContactManifold(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB, ref ConvexContactManifold manifold)
    {
        return true;
    }

    public void Dispose()
    {
        Properties.Dispose();
        Characters.Dispose();
    }
}
