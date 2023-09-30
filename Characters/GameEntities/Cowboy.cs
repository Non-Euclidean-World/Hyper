using BepuPhysics;
using Character.Characters;
using OpenTK.Mathematics;
using Physics.Collisions;
using Physics.TypingUtils;

namespace Character.GameEntities;

public class Cowboy : Humanoid
{
    private Vector3 _goalPosition;

    private bool _isMoving = false;

    private float _idleTime = 0;
    
    private float _moveTime = 0;

    public Cowboy(PhysicalCharacter physicalCharacter) : base(
        new Model(CowboyResources.Instance, 0.04f, new Vector3(0, -5, 0)), physicalCharacter)
    {
        _goalPosition = Conversions.ToOpenTKVector(PhysicalCharacter.Pose.Position);
    }
    
    public override void UpdateCharacterGoals(Simulation simulation, float time)
    {
        if (_isMoving == false)
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
        }
        
        ViewDirection = _goalPosition - Conversions.ToOpenTKVector(PhysicalCharacter.Pose.Position);
        var movementDirection = System.Numerics.Vector2.UnitY;
        if (ViewDirection is { X: < 0.1f, Z: < 0.1f })
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
        PhysicalCharacter.UpdateCharacterGoals(simulation, Conversions.ToNumericsVector(ViewDirection), time, false, false, movementDirection);
    }

    private void Run() => Character.Animator.Play(0);
    
    private void Idle() => Character.Animator.Reset();
}