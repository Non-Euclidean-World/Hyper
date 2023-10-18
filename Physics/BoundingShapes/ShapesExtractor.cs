using System.Numerics;
using System.Runtime.CompilerServices;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Collections;
using BepuUtilities.Memory;
using Physics.Collisions;

namespace Physics.BoundingShapes;
public class ShapesExtractor : IDisposable
{
    private readonly Simulation _simulation;

    private readonly BufferPool _pool;

    public QuickList<BoundingBox> Boxes;

    public QuickList<BoundingCapsule> Capsules;

    public QuickList<BoundingCylinder> Cylinders;

    public ShapesExtractor(Simulation simulation)
    {
        _simulation = simulation;

        const int initialCapacity = 128;
        _pool = new BufferPool();
        Boxes = new QuickList<BoundingBox>(initialCapacity, _pool);
        Capsules = new QuickList<BoundingCapsule>(initialCapacity, _pool);
        Cylinders = new QuickList<BoundingCylinder>(initialCapacity, _pool);
    }

    public void ClearCache()
    {
        Boxes.Count = 0;
        Capsules.Count = 0;
        Cylinders.Count = 0;
    }

    public void Dispose()
    {
        Boxes.Dispose(_pool);
        Capsules.Dispose(_pool);
        Cylinders.Dispose(_pool);
    }

    public void AddShapes(Dictionary<BodyHandle, ISimulationMember> simulationMembers)
    {
        for (int setIndex = 0; setIndex < _simulation.Bodies.Sets.Length; setIndex++)
        {
            ref var set = ref _simulation.Bodies.Sets[setIndex];
            if (set.Allocated && set.Count > 0)
            {
                for (int indexInSet = 0; indexInSet < set.Count; indexInSet++)
                {
                    AddShape(setIndex, indexInSet, simulationMembers);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddShape(int setIndex, int indexInSet, Dictionary<BodyHandle, ISimulationMember> simulationMembers)
    {
        ref var set = ref _simulation.Bodies.Sets[setIndex];
        ref var state = ref set.SolverStates[indexInSet];
        var handle = set.IndexToHandle[indexInSet];
        if (!simulationMembers.ContainsKey(handle))
            return;

        int sphereId = simulationMembers[handle].CurrentSphereId;

        Vector3 color;
        if (setIndex == 0)
        {
            color = new Vector3(0, 1, 0);
        }
        else
        {
            color = new Vector3(1, 0, 0);
        }

        AddShape(set.Collidables[indexInSet].Shape, state.Motion.Pose, color, sphereId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe void AddShape(TypedIndex shapeIndex, RigidPose pose, Vector3 color, int sphereId)
    {
        if (shapeIndex.Exists)
        {
            _simulation.Shapes[shapeIndex.Type].GetShapeData(shapeIndex.Index, out var shapeData, out _);
            AddShape(shapeData, shapeIndex.Type, pose, color, sphereId);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe void AddShape(void* shapeData, int shapeType, RigidPose pose, Vector3 color, int sphereId)
    {
        switch (shapeType)
        {
            case Capsule.Id:
                {
                    BoundingCapsule boundingCapsule;
                    boundingCapsule.Pose = pose;
                    ref var capsule = ref Unsafe.AsRef<Capsule>(shapeData);
                    boundingCapsule.Radius = capsule.Radius;
                    boundingCapsule.HalfLength = capsule.HalfLength;
                    boundingCapsule.SphereId = sphereId;
                    boundingCapsule.Color = color;
                    Capsules.Add(boundingCapsule, _pool);
                }
                break;
            case Box.Id:
                {
                    BoundingBox boundingBox;
                    boundingBox.Pose = pose;
                    ref var box = ref Unsafe.AsRef<Box>(shapeData);
                    boundingBox.HalfLength = box.HalfLength;
                    boundingBox.HalfWidth = box.HalfWidth;
                    boundingBox.HalfHeight = box.HalfHeight;
                    boundingBox.SphereId = sphereId;
                    boundingBox.Color = color;
                    Boxes.Add(boundingBox, _pool);
                }
                break;
            case Cylinder.Id:
                {
                    BoundingCylinder boundingCylinder;
                    boundingCylinder.Pose = pose;
                    ref var cylinder = ref Unsafe.AsRef<Cylinder>(shapeData);
                    boundingCylinder.HalfLength = cylinder.HalfLength;
                    boundingCylinder.Radius = cylinder.Radius;
                    boundingCylinder.SphereId = sphereId;
                    boundingCylinder.Color = color;
                    Cylinders.Add(boundingCylinder, _pool);
                }
                break;
            case Compound.Id:
                {
                    AddCompoundChildren(ref Unsafe.AsRef<Compound>(shapeData).Children, pose, color, sphereId);
                }
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private void AddCompoundChildren(ref Buffer<CompoundChild> children, RigidPose pose, Vector3 color, int sphereId)
    {
        for (int i = 0; i < children.Length; i++)
        {
            ref var child = ref children[i];
            Compound.GetWorldPose(child.LocalPose, pose, out var childPose);
            AddShape(child.ShapeIndex, childPose, color, sphereId);
        }
    }
}
