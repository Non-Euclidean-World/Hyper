// Copyright The Authors of bepuphysics2

using BepuPhysics;

namespace Hyper.Collisions;
internal class SimpleCarController
{
    private float _steeringAngle;

    public float SteeringAngle { get => _steeringAngle; }

    public float SteeringSpeed;
    public float MaximumSteeringAngle;

    public float ForwardSpeed;
    public float ForwardForce;
    public float ZoomMultiplier;
    public float BackwardSpeed;
    public float BackwardForce;
    public float IdleForce;
    public float BrakeForce;
    public float WheelBaseLength;
    public float WheelBaseWidth;
    /// <summary>
    /// Fraction of Ackerman steering angle to apply to wheels. Using 0 does not modify the the steering angle at all, leaving the wheels pointed exactly along the steering angle, while 1 uses the full Ackerman angle.
    /// </summary>
    public float AckermanSteering;

    //Track the previous state to force wakeups if the constraint targets have changed.
    private float _previousTargetSpeed;
    private float _previousTargetForce;

    public SimpleCarController(
        float forwardSpeed, float forwardForce, float zoomMultiplier, float backwardSpeed, float backwardForce, float idleForce, float brakeForce,
        float steeringSpeed, float maximumSteeringAngle, float wheelBaseLength, float wheelBaseWidth, float ackermanSteering)
    {
        ForwardSpeed = forwardSpeed;
        ForwardForce = forwardForce;
        ZoomMultiplier = zoomMultiplier;
        BackwardSpeed = backwardSpeed;
        BackwardForce = backwardForce;
        IdleForce = idleForce;
        BrakeForce = brakeForce;
        SteeringSpeed = steeringSpeed;
        MaximumSteeringAngle = maximumSteeringAngle;
        WheelBaseLength = wheelBaseLength;
        WheelBaseWidth = wheelBaseWidth;
        AckermanSteering = ackermanSteering;

        _steeringAngle = 0;
        _previousTargetForce = 0;
        _previousTargetSpeed = 0;
    }

    public void Update(Simulation simulation, SimpleCar car, float dt, float targetSteeringAngle, float targetSpeedFraction, bool zoom, bool brake)
    {
        var steeringAngleDifference = targetSteeringAngle - _steeringAngle;
        var maximumChange = SteeringSpeed * dt;
        var steeringAngleChange = MathF.Min(maximumChange, MathF.Max(-maximumChange, steeringAngleDifference));
        var previousSteeringAngle = _steeringAngle;

        _steeringAngle = MathF.Min(MaximumSteeringAngle, MathF.Max(-MaximumSteeringAngle, _steeringAngle + steeringAngleChange));
        if (_steeringAngle != previousSteeringAngle)
        {
            float leftSteeringAngle;
            float rightSteeringAngle;

            var steeringAngleAbs = MathF.Abs(_steeringAngle);

            if (AckermanSteering > 0 && steeringAngleAbs > 1e-6)
            {
                var turnRadius = MathF.Abs(WheelBaseLength * MathF.Tan(MathF.PI * 0.5f - steeringAngleAbs));
                var wheelBaseHalfWidth = WheelBaseWidth * 0.5f;
                if (_steeringAngle > 0)
                {
                    rightSteeringAngle = MathF.Atan(WheelBaseLength / (turnRadius - wheelBaseHalfWidth));
                    rightSteeringAngle = _steeringAngle + (rightSteeringAngle - steeringAngleAbs) * AckermanSteering;

                    leftSteeringAngle = MathF.Atan(WheelBaseLength / (turnRadius + wheelBaseHalfWidth));
                    leftSteeringAngle = _steeringAngle + (leftSteeringAngle - steeringAngleAbs) * AckermanSteering;
                }
                else
                {
                    rightSteeringAngle = MathF.Atan(WheelBaseLength / (turnRadius + wheelBaseHalfWidth));
                    rightSteeringAngle = _steeringAngle - (rightSteeringAngle - steeringAngleAbs) * AckermanSteering;

                    leftSteeringAngle = MathF.Atan(WheelBaseLength / (turnRadius - wheelBaseHalfWidth));
                    leftSteeringAngle = _steeringAngle - (leftSteeringAngle - steeringAngleAbs) * AckermanSteering;
                }
            }
            else
            {
                leftSteeringAngle = _steeringAngle;
                rightSteeringAngle = _steeringAngle;
            }

            //By guarding the constraint modifications behind a state test, we avoid waking up the car every single frame.
            //(We could have also used the ApplyDescriptionWithoutWaking function and then explicitly woke the car up when changes occur.)
            car.Steer(simulation, car.FrontLeftWheel, leftSteeringAngle);
            car.Steer(simulation, car.FrontRightWheel, rightSteeringAngle);
        }
        float newTargetSpeed, newTargetForce;
        bool allWheels;
        if (brake)
        {
            newTargetSpeed = 0;
            newTargetForce = BrakeForce;
            allWheels = true;
        }
        else if (targetSpeedFraction > 0)
        {
            newTargetForce = zoom ? ForwardForce * ZoomMultiplier : ForwardForce;
            newTargetSpeed = targetSpeedFraction * (zoom ? ForwardSpeed * ZoomMultiplier : ForwardSpeed);
            allWheels = false;
        }
        else if (targetSpeedFraction < 0)
        {
            newTargetForce = zoom ? BackwardForce * ZoomMultiplier : BackwardForce;
            newTargetSpeed = targetSpeedFraction * (zoom ? BackwardSpeed * ZoomMultiplier : BackwardSpeed);
            allWheels = false;
        }
        else
        {
            newTargetForce = IdleForce;
            newTargetSpeed = 0;
            allWheels = true;
        }
        if (_previousTargetSpeed != newTargetSpeed || _previousTargetForce != newTargetForce)
        {
            _previousTargetSpeed = newTargetSpeed;
            _previousTargetForce = newTargetForce;
            SimpleCar.SetSpeed(simulation, car.FrontLeftWheel, newTargetSpeed, newTargetForce);
            SimpleCar.SetSpeed(simulation, car.FrontRightWheel, newTargetSpeed, newTargetForce);
            if (allWheels)
            {
                SimpleCar.SetSpeed(simulation, car.BackLeftWheel, newTargetSpeed, newTargetForce);
                SimpleCar.SetSpeed(simulation, car.BackRightWheel, newTargetSpeed, newTargetForce);
            }
            else
            {
                SimpleCar.SetSpeed(simulation, car.BackLeftWheel, 0, 0);
                SimpleCar.SetSpeed(simulation, car.BackRightWheel, 0, 0);
            }
        }
    }
}
