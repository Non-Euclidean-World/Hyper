// Copyright The Authors of bepuphysics2

using System.Diagnostics;
using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities;
using Common;
using Common.Meshes;
using OpenTK.Graphics.OpenGL4;

namespace Physics.Collisions;

public class PhysicalCharacter
{
    public RigidPose Pose { get; private set; }

    public BodyHandle BodyHandle { get => _bodyHandle; }

    public Body BoundingBox { get; private set; }

    private BodyHandle _bodyHandle;
    private readonly CharacterControllers _characters;
    private readonly float _speed;
    private Capsule _shape;

    public PhysicalCharacter(CharacterControllers characters, CollidableProperty<SimulationProperties> properties, Vector3 initialPosition,
        float minimumSpeculativeMargin, float mass, float maximumHorizontalForce, float maximumVerticalGlueForce,
        float jumpVelocity, float speed, float maximumSlope = MathF.PI * 0.25f)
    {
        _characters = characters;
        Capsule capsule = new Capsule(1, 2);
        _shape = capsule;
        var shapeIndex = characters.Simulation.Shapes.Add(_shape);

        //Because characters are dynamic, they require a defined BodyInertia. For the purposes of the demos, we don't want them to rotate or fall over, so the inverse inertia tensor is left at its default value of all zeroes.
        //This is effectively equivalent to giving it an infinite inertia tensor- in other words, no torque will cause it to rotate.
        _bodyHandle = characters.Simulation.Bodies.Add(
            BodyDescription.CreateDynamic(initialPosition, new BodyInertia { InverseMass = 1f / mass },
            new(shapeIndex, minimumSpeculativeMargin, float.MaxValue, ContinuousDetection.Passive), activity: _shape.Radius * 0.000002f));
        ref var physicalCharacter = ref characters.AllocateCharacter(_bodyHandle);
        physicalCharacter.LocalUp = new Vector3(0, 1, 0);
        physicalCharacter.CosMaximumSlope = MathF.Cos(maximumSlope);
        physicalCharacter.JumpVelocity = jumpVelocity;
        physicalCharacter.MaximumVerticalForce = maximumVerticalGlueForce;
        physicalCharacter.MaximumHorizontalForce = maximumHorizontalForce;
        physicalCharacter.MinimumSupportDepth = _shape.Radius * -0.01f;
        physicalCharacter.MinimumSupportContinuationDepth = -minimumSpeculativeMargin;

        ref var bodyProperties = ref properties.Allocate(_bodyHandle);
        bodyProperties = new SimulationProperties { Friction = 2f, Filter = new SubgroupCollisionFilter(_bodyHandle.Value, 0) };

        _speed = speed;
        BoundingBox = new Body(BoxMesh.Create(new OpenTK.Mathematics.Vector3(_shape.Radius * 2, _shape.Length + _shape.Radius * 2, _shape.Radius * 2)));
    }

    public void UpdateCharacterGoals(Simulation simulation, Vector3 viewDirection, float simulationTimestepDuration, bool tryJump, bool sprint, Vector2 movementDirection)
    {
        var movementDirectionLengthSquared = movementDirection.LengthSquared();
        if (movementDirectionLengthSquared > 0)
        {
            movementDirection /= MathF.Sqrt(movementDirectionLengthSquared);
        }

        ref var character = ref _characters.GetCharacterByBodyHandle(_bodyHandle);
        character.TryJump = tryJump;
        var characterBody = new BodyReference(_bodyHandle, _characters.Simulation.Bodies);
        var effectiveSpeed = sprint ? _speed * 1.75f : _speed;
        var newTargetVelocity = movementDirection * effectiveSpeed;
        //var viewDirection = Conversions.ToNumericsVector(camera.Front);
        //Modifying the character's raw data does not automatically wake the character up, so we do so explicitly if necessary.
        //If you don't explicitly wake the character up, it won't respond to the changed motion goals.
        //(You can also specify a negative deactivation threshold in the BodyActivityDescription to prevent the character from sleeping at all.)
        if (!characterBody.Awake &&
            ((character.TryJump && character.Supported) ||
            newTargetVelocity != character.TargetVelocity ||
            (newTargetVelocity != Vector2.Zero && character.ViewDirection != viewDirection)))
        {
            _characters.Simulation.Awakener.AwakenBody(character.BodyHandle);
        }
        character.TargetVelocity = newTargetVelocity;
        character.ViewDirection = viewDirection;

        //The character's motion constraints aren't active while the character is in the air, so if we want air control, we'll need to apply it ourselves.
        //(You could also modify the constraints to do this, but the robustness of solved constraints tends to be a lot less important for air control.)
        //There isn't any one 'correct' way to implement air control- it's a nonphysical gameplay thing, and this is just one way to do it.
        //Note that this permits accelerating along a particular direction, and never attempts to slow down the character.
        //This allows some movement quirks common in some game character controllers.
        //Consider what happens if, starting from a standstill, you accelerate fully along X, then along Z- your full velocity magnitude will be sqrt(2) * maximumAirSpeed.
        //Feel free to try alternative implementations. Again, there is no one correct approach.
        if (!character.Supported && movementDirectionLengthSquared > 0)
        {
            QuaternionEx.Transform(character.LocalUp, characterBody.Pose.Orientation, out var characterUp);
            var characterRight = Vector3.Cross(character.ViewDirection, characterUp);
            var rightLengthSquared = characterRight.LengthSquared();
            if (rightLengthSquared > 1e-10f)
            {
                characterRight /= MathF.Sqrt(rightLengthSquared);
                var characterForward = Vector3.Cross(characterUp, characterRight);
                var worldMovementDirection = characterRight * movementDirection.X + characterForward * movementDirection.Y;
                var currentVelocity = Vector3.Dot(characterBody.Velocity.Linear, worldMovementDirection);
                //We'll arbitrarily set air control to be a fraction of supported movement's speed/force.
                const float airControlForceScale = .2f;
                const float airControlSpeedScale = .2f;
                var airAccelerationDt = characterBody.LocalInertia.InverseMass * character.MaximumHorizontalForce * airControlForceScale * simulationTimestepDuration;
                var maximumAirSpeed = effectiveSpeed * airControlSpeedScale;
                var targetVelocity = MathF.Min(currentVelocity + airAccelerationDt, maximumAirSpeed);
                //While we shouldn't allow the character to continue accelerating in the air indefinitely, trying to move in a given direction should never slow us down in that direction.
                var velocityChangeAlongMovementDirection = MathF.Max(0, targetVelocity - currentVelocity);
                characterBody.Velocity.Linear += worldMovementDirection * velocityChangeAlongMovementDirection;
                Debug.Assert(characterBody.Awake, "Velocity changes don't automatically update objects; the character should have already been woken up before applying air control.");
            }
        }

        var body = new BodyReference(_bodyHandle, simulation.Bodies);

        float angle = MathF.Atan2(viewDirection.X, viewDirection.Z);
        body.Pose.Orientation = QuaternionEx.CreateFromAxisAngle(Vector3.UnitY, angle);

        BoundingBox.RigidPose = body.Pose;
        Pose = body.Pose;
    }

    /// <summary>
    /// Disregards physics of the character and changes its pose.
    /// The intention is that this method should only be used for teleportation functionality
    /// </summary>
    /// <param name="simulation"></param>
    /// <param name="pose"></param>
    public void ForcePoseChange(Simulation simulation, RigidPose pose, Func<Vector3, Vector3> velocityTransform) // TODO we need to also change the velocity vector
    {
        var body = new BodyReference(_bodyHandle, simulation.Bodies);
        body.Pose = pose;
        body.Velocity.Linear = velocityTransform(body.Velocity.Linear);
    }

    /// <summary>
    /// Removes the character's body from the simulation and the character from the associated characters set.
    /// </summary>
    public void Dispose()
    {
        _characters.Simulation.Shapes.Remove(new BodyReference(_bodyHandle, _characters.Simulation.Bodies).Collidable.Shape);
        _characters.Simulation.Bodies.Remove(_bodyHandle);
        _characters.RemoveCharacterByBodyHandle(_bodyHandle);
    }

    public void RenderBoundingBox(Shader shaderBoundingBox, float scale, float curve, OpenTK.Mathematics.Vector3 cameraPosition)
    {
        TurnOnWireframe();
        BoundingBox.RenderFullDescription(shaderBoundingBox, scale, curve, cameraPosition);
        TurnOffWireframe();
    }

    private static void TurnOnWireframe()
    {
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
    }

    private static void TurnOffWireframe()
    {
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
    }
}