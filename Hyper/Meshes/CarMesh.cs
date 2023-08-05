using BepuPhysics;
using Hyper.Collisions;
using OpenTK.Mathematics;

namespace Hyper.Meshes;
internal class CarMesh
{
    public Mesh UpperPart { get; private set; }
    public Mesh LowerPart { get; private set; }
    public Mesh BackLeftWheel { get; private set; }
    public Mesh BackRightWheel { get; private set; }
    public Mesh FrontLeftWheel { get; private set; }
    public Mesh FrontRightWheel { get; private set; }
    public float WheelRadius { get; private set; }
    public float WheelWidth { get; private set; }

    public Vector3 Position { get; private set; }

    public Vector3 UpperPartOffset { get; private set; }

    public CarMesh(Vector3 upperPart, Vector3 lowerPart, Vector3 upperPartOffset, float wheelRadius, float wheelWidth)
    {
        UpperPart = BoxMesh.Create(upperPart);
        WheelRadius = wheelRadius;
        WheelWidth = wheelWidth;
        UpperPartOffset = upperPartOffset;
        LowerPart = BoxMesh.Create(lowerPart);
        BackLeftWheel = BoxMesh.Create(new Vector3(wheelRadius, wheelRadius, wheelWidth));
        BackRightWheel = BoxMesh.Create(new Vector3(wheelRadius, wheelRadius, wheelWidth));
        FrontLeftWheel = BoxMesh.Create(new Vector3(wheelRadius, wheelRadius, wheelWidth));
        FrontRightWheel = BoxMesh.Create(new Vector3(wheelRadius, wheelRadius, wheelWidth));
    }

    public void Update(RigidPose bodyPose, RigidPose rearLeftWheelPose, RigidPose rearRightWheelPose, RigidPose frontLeftWheelPose, RigidPose frontRightWheelPose)
    {
        LowerPart.RigidPose = bodyPose;
        UpperPart.RigidPose = new RigidPose(bodyPose.Position + TypingUtils.ToNumericsVector(UpperPartOffset), bodyPose.Orientation);
        BackLeftWheel.RigidPose = rearLeftWheelPose;
        BackRightWheel.RigidPose = rearRightWheelPose;
        FrontLeftWheel.RigidPose = frontLeftWheelPose;
        FrontRightWheel.RigidPose = frontRightWheelPose;
    }

    public void Render(Shader shader, float scale, Vector3 cameraPosition)
    {
        LowerPart.RenderFullDescription(shader, scale, cameraPosition);
        BackLeftWheel.RenderFullDescription(shader, scale, cameraPosition);
        BackRightWheel.RenderFullDescription(shader, scale, cameraPosition);
        FrontLeftWheel.RenderFullDescription(shader, scale, cameraPosition);
        FrontRightWheel.RenderFullDescription(shader, scale, cameraPosition);
    }
}
