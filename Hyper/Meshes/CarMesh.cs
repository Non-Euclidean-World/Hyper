using BepuPhysics;
using OpenTK.Mathematics;

namespace Hyper.Meshes;
internal class CarMesh
{
    public Mesh LowerPart { get; private set; }
    public Mesh BackLeftWheel { get; private set; }
    public Mesh BackRightWheel { get; private set; }
    public Mesh FrontLeftWheel { get; private set; }
    public Mesh FrontRightWheel { get; private set; }
    public float WheelRadius { get; private set; }
    public float WheelWidth { get; private set; }

    public CarMesh(Vector3 lowerPart, float wheelRadius, float wheelWidth)
    {
        WheelRadius = wheelRadius;
        WheelWidth = wheelWidth;
        LowerPart = BoxMesh.Create(lowerPart);
        float wheelDiameter = 2 * wheelRadius;
        BackLeftWheel = BoxMesh.Create(new Vector3(wheelDiameter, wheelWidth, wheelDiameter));
        BackRightWheel = BoxMesh.Create(new Vector3(wheelDiameter, wheelWidth, wheelDiameter));
        FrontLeftWheel = BoxMesh.Create(new Vector3(wheelDiameter, wheelWidth, wheelDiameter));
        FrontRightWheel = BoxMesh.Create(new Vector3(wheelDiameter, wheelWidth, wheelDiameter));
    }

    public void Update(RigidPose bodyPose, RigidPose rearLeftWheelPose, RigidPose rearRightWheelPose, RigidPose frontLeftWheelPose, RigidPose frontRightWheelPose)
    {
        LowerPart.RigidPose = bodyPose;
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
