using BepuPhysics;
using Common;
using Common.Meshes;
using OpenTK.Mathematics;
using Physics;

namespace Character.Vehicles;

public class CarMesh
{
    public Body LowerPart { get; private set; }
    public Body BackLeftWheel { get; private set; }
    public Body BackRightWheel { get; private set; }
    public Body FrontLeftWheel { get; private set; }
    public Body FrontRightWheel { get; private set; }
    public float WheelRadius { get; private set; }
    public float WheelWidth { get; private set; }

    public CarMesh(Vector3 lowerPart, float wheelRadius, float wheelWidth)
    {
        WheelRadius = wheelRadius;
        WheelWidth = wheelWidth;
        LowerPart = new Body(BoxMesh.Create(lowerPart));
        float wheelDiameter = 2 * wheelRadius;
        BackLeftWheel = new Body(BoxMesh.Create(new Vector3(wheelDiameter, wheelWidth, wheelDiameter)));
        BackRightWheel = new Body(BoxMesh.Create(new Vector3(wheelDiameter, wheelWidth, wheelDiameter)));
        FrontLeftWheel = new Body(BoxMesh.Create(new Vector3(wheelDiameter, wheelWidth, wheelDiameter)));
        FrontRightWheel = new Body(BoxMesh.Create(new Vector3(wheelDiameter, wheelWidth, wheelDiameter)));
    }

    public void Update(RigidPose bodyPose, RigidPose rearLeftWheelPose, RigidPose rearRightWheelPose, RigidPose frontLeftWheelPose, RigidPose frontRightWheelPose)
    {
        LowerPart.RigidPose = bodyPose;
        BackLeftWheel.RigidPose = rearLeftWheelPose;
        BackRightWheel.RigidPose = rearRightWheelPose;
        FrontLeftWheel.RigidPose = frontLeftWheelPose;
        FrontRightWheel.RigidPose = frontRightWheelPose;
    }

    public void Render(Shader shader, float scale, float curve, Vector3 cameraPosition)
    {
        LowerPart.RenderFullDescription(shader, scale, curve, cameraPosition);
        BackLeftWheel.RenderFullDescription(shader, scale, curve, cameraPosition);
        BackRightWheel.RenderFullDescription(shader, scale, curve, cameraPosition);
        FrontLeftWheel.RenderFullDescription(shader, scale, curve, cameraPosition);
        FrontRightWheel.RenderFullDescription(shader, scale, curve, cameraPosition);
    }
}
