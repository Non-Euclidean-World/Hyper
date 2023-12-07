using BepuPhysics;
using Character.Characters;
using OpenTK.Mathematics;
using Physics.Collisions;
using Physics.TypingUtils;

namespace Character.GameEntities;

public class AstronautBot : Humanoid
{
    private Vector3 _goalPosition;

    private bool _isMoving;

    private float _idleTime;

    private float _moveTime;

    private Vector3i[]? _sphereCenters;

    public AstronautBot(PhysicalCharacter physicalCharacter, Vector3i[]? sphereCenters = null) : base(
        new Model(AstronautBotResources.Instance, localScale: 0.45f, localTranslation: new Vector3(0, -4.4f, 0)), physicalCharacter)
    {
        _goalPosition = Conversions.ToOpenTKVector(PhysicalCharacter.Pose.Position);
        _sphereCenters = sphereCenters;
    }

    public override void UpdateCharacterGoals(Simulation simulation, float time)
    {
        /*if (_isMoving == false)
        {
            _idleTime += time;
            if (_idleTime > _moveTime)
            {
                _isMoving = true;
                _idleTime = 0;
                Random random = new();
                _moveTime = random.Next(5);
                _goalPosition = Conversions.ToOpenTKVector(PhysicalCharacter.Pose.Position) + new Vector3(random.Next(-10, 10), 0, random.Next(-10, 10));
            }
        }*/

        ViewDirection = AdjustSphere(_goalPosition) - Conversions.ToOpenTKVector(PhysicalCharacter.Pose.Position);
        var movementDirection = System.Numerics.Vector2.Zero;
        if (true)
        {
            movementDirection = System.Numerics.Vector2.Zero;
            ViewDirection = Vector3.Zero;
            _isMoving = false;
        }

        if (_isMoving)
        {
            Run();
        }
        else
        {
            Idle();
        }
        PhysicalCharacter.UpdateCharacterGoals(simulation,
            Conversions.ToNumericsVector(ViewDirection),
            time,
            tryJump: false,
            sprint: false,
            movementDirection);
    }

    private Vector3 AdjustSphere(Vector3 position)
    {
        if (_sphereCenters == null)
            return position;

        return _sphereCenters[CurrentSphereId] + position;
    }

    private void Run() => Character.Animator.Play(0);

    private void Idle() => Character.Animator.Reset();
}