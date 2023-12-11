using BepuPhysics;
using Character;
using Character.Characters;
using Character.Projectiles;
using OpenTK.Mathematics;
using Physics.Collisions;
using Physics.TypingUtils;

namespace Hyper.GameEntities;

public class AstronautBot : Humanoid
{
    private Vector3 _goalPosition;

    private bool _isMoving;

    private float _idleTime;

    private float _moveTime;

    private const float MaxNotShootingTime = 4;

    private float _notShootingTime;

    private float _shootTime = 4;

    private readonly Vector3i[]? _sphereCenters;

    private bool _isFriendly;

    private static readonly float VisibilityRadius = 15f;

    private Random _random = new();

    public AstronautBot(PhysicalCharacter physicalCharacter, Vector3i[]? sphereCenters = null) : base(
        new Model(AstronautBotResources.Instance, localScale: 0.45f, localTranslation: new Vector3(0, -4.4f, 0)), physicalCharacter)
    {
        _goalPosition = Conversions.ToOpenTKVector(PhysicalCharacter.Pose.Position);
        _sphereCenters = sphereCenters;
        _isFriendly = _random.Next(0, 2) == 0;
    }

    public override void UpdateCharacterGoals(Simulation simulation, float time)
    {
        throw new NotImplementedException();
    }

    internal void UpdateCharacterGoals(Simulation simulation, float time, Scene scene)
    {
        if (!_isFriendly && !scene.Player.Hidden && scene.Player.CurrentSphereId == CurrentSphereId)
        {
            UpdateBotGoalsWhenHostile(simulation, time, scene);
        }
        else
        {
            UpdateBotGoalsWhenFriendly(simulation, time);
        }
    }

    private void UpdateBotGoalsWhenFriendly(Simulation simulation, float time)
    {
        if (_isMoving == false)
        {
            _idleTime += time;
            if (_idleTime > _moveTime)
            {
                _isMoving = true;
                _idleTime = 0;
                _moveTime = _random.Next(5);
                _goalPosition = Conversions.ToOpenTKVector(PhysicalCharacter.Pose.Position) + new Vector3(_random.Next(-10, 10), 0, _random.Next(-10, 10));
            }
        }

        ViewDirection = AdjustSphere(_goalPosition) - Conversions.ToOpenTKVector(PhysicalCharacter.Pose.Position);
        var movementDirection = System.Numerics.Vector2.UnitY;
        if (FlatLength(ViewDirection) < 0.2)
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

    private void UpdateBotGoalsWhenHostile(Simulation simulation, float time, Scene scene)
    {
        var player = scene.Player;
        if (System.Numerics.Vector3.Distance(PhysicalCharacter.Pose.Position, player.PhysicalCharacter.Pose.Position) > VisibilityRadius)
        {
            UpdateBotGoalsWhenFriendly(simulation, time);
            return;
        }

        _goalPosition = Conversions.ToOpenTKVector(player.PhysicalCharacter.Pose.Position);
        ViewDirection = _goalPosition // no adjustment for spherical space because bot & player are in the same sphere
            - Conversions.ToOpenTKVector(PhysicalCharacter.Pose.Position);
        _isMoving = true;
        var movementDirection = System.Numerics.Vector2.UnitY;

        if (FlatLength(ViewDirection) < 2.5)
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

        _notShootingTime += time;
        if (_notShootingTime >= _shootTime)
        {
            CreateProjectile(scene);
            _shootTime = _random.NextSingle() * MaxNotShootingTime;
            _notShootingTime = 0;
        }

        PhysicalCharacter.UpdateCharacterGoals(simulation,
            Conversions.ToNumericsVector(ViewDirection),
            time,
            tryJump: false,
            sprint: false,
            movementDirection);
    }

    private void CreateProjectile(Scene scene)
    {
        var normalizedView = Conversions.ToNumericsVector(Vector3.NormalizeFast(ViewDirection));
        var q = Helpers.CreateQuaternionFromTwoVectors(System.Numerics.Vector3.UnitX, normalizedView);
        var projectile = Projectile.CreateStandardProjectile(scene.SimulationManager.Simulation,
            scene.SimulationManager.Properties,
            new RigidPose(PhysicalCharacter.Pose.Position + normalizedView * 3 + System.Numerics.Vector3.UnitY * 1.5f, q),
            normalizedView * 20,
            new ProjectileMesh(0.4f, 0.1f, 0.1f),
            lifeTime: 5,
            CurrentSphereId); // let's throw some refrigerators
        scene.Projectiles.Add(projectile);
        scene.SimulationMembers.Add(projectile);
    }

    private static float FlatLength(Vector3 v)
        => v.Xz.LengthFast;

    private Vector3 AdjustSphere(Vector3 position)
    {
        if (_sphereCenters == null)
            return position;

        return _sphereCenters[CurrentSphereId] + position;
    }

    private void Run() => Character.Animator.Play(0);

    private void Idle() => Character.Animator.Reset();
}