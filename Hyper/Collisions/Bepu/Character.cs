using System.Diagnostics;
using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities;
using Hyper.Meshes;
using Hyper.TypingUtils;
using OpenTK.Graphics.OpenGL4;

namespace Hyper.Collisions.Bepu;
internal class Character
{
    BodyHandle bodyHandle;
    CharacterControllers characters;
    float speed;
    Capsule shape;


    PlayerData.Player _player;
    public PlayerData.Player Player { get => _player; }

    public Meshes.Mesh Mesh { get => _mesh; }
    private Meshes.Mesh _mesh;

    public BodyHandle BodyHandle { get { return bodyHandle; } }

    public Character(CharacterControllers characters, Vector3 initialPosition,
        float minimumSpeculativeMargin, float mass, float maximumHorizontalForce, float maximumVerticalGlueForce,
        float jumpVelocity, float speed, float maximumSlope = MathF.PI * 0.25f)
    {
        this.characters = characters;
        _player = new PlayerData.Player();
        Capsule capsule = new Capsule(1, 2);
        this.shape = capsule;
        var shapeIndex = characters.Simulation.Shapes.Add(shape);

        //Because characters are dynamic, they require a defined BodyInertia. For the purposes of the demos, we don't want them to rotate or fall over, so the inverse inertia tensor is left at its default value of all zeroes.
        //This is effectively equivalent to giving it an infinite inertia tensor- in other words, no torque will cause it to rotate.
        bodyHandle = characters.Simulation.Bodies.Add(
            BodyDescription.CreateDynamic(initialPosition, new BodyInertia { InverseMass = 1f / mass },
            new(shapeIndex, minimumSpeculativeMargin, float.MaxValue, ContinuousDetection.Passive), activity: shape.Radius * 0.0002f));
        ref var character = ref characters.AllocateCharacter(bodyHandle);
        character.LocalUp = new Vector3(0, 1, 0);
        character.CosMaximumSlope = MathF.Cos(maximumSlope);
        character.JumpVelocity = jumpVelocity;
        character.MaximumVerticalForce = maximumVerticalGlueForce;
        character.MaximumHorizontalForce = maximumHorizontalForce;
        character.MinimumSupportDepth = shape.Radius * -0.01f;
        character.MinimumSupportContinuationDepth = -minimumSpeculativeMargin;
        this.speed = speed;

        _mesh = BoxMesh.Create(new OpenTK.Mathematics.Vector3(shape.Radius * 2, shape.Length + shape.Radius * 2, shape.Radius * 2));
    }

    public void UpdateCharacterGoals(Simulation simulation, Camera camera, float simulationTimestepDuration, bool tryJump, bool sprint, Vector2 movementDirection)
    {
        var movementDirectionLengthSquared = movementDirection.LengthSquared();
        if (movementDirectionLengthSquared > 0)
        {
            movementDirection /= MathF.Sqrt(movementDirectionLengthSquared);
        }

        ref var character = ref characters.GetCharacterByBodyHandle(bodyHandle);
        character.TryJump = tryJump;
        var characterBody = new BodyReference(bodyHandle, characters.Simulation.Bodies);
        var effectiveSpeed = sprint ? speed * 1.75f : speed;
        var newTargetVelocity = movementDirection * effectiveSpeed;
        var viewDirection = Conversions.ToNumericsVector(camera.Front);
        //Modifying the character's raw data does not automatically wake the character up, so we do so explicitly if necessary.
        //If you don't explicitly wake the character up, it won't respond to the changed motion goals.
        //(You can also specify a negative deactivation threshold in the BodyActivityDescription to prevent the character from sleeping at all.)
        if (!characterBody.Awake &&
            ((character.TryJump && character.Supported) ||
            newTargetVelocity != character.TargetVelocity ||
            (newTargetVelocity != Vector2.Zero && character.ViewDirection != viewDirection)))
        {
            characters.Simulation.Awakener.AwakenBody(character.BodyHandle);
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

        var body = new BodyReference(bodyHandle, simulation.Bodies);


        _player.Update(body.Pose);


        _mesh.RigidPose = body.Pose;


    }

    // TODO this should be a method in mesh cf. simple car
    public void RenderCharacterMesh(Shader shader, Shader shaderBoundingBox, float scale, OpenTK.Mathematics.Vector3 cameraPosition)
    {

        _player.Render(shader, scale, cameraPosition);

        TurnOnWireframe();
        _mesh.RenderFullDescription(shaderBoundingBox, scale, cameraPosition);
        TurnOffWireframe();
    }


    /// <summary>
    /// Removes the character's body from the simulation and the character from the associated characters set.
    /// </summary>
    public void Dispose()
    {
        characters.Simulation.Shapes.Remove(new BodyReference(bodyHandle, characters.Simulation.Bodies).Collidable.Shape);
        characters.Simulation.Bodies.Remove(bodyHandle);
        characters.RemoveCharacterByBodyHandle(bodyHandle);
    }

    private void TurnOnWireframe()
    {
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
    }

    private void TurnOffWireframe()
    {
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
    }

}
