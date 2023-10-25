using BepuPhysics;
using Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Physics.TypingUtils;

namespace Character.Vehicles;
public class FourWheeledCarModel
{
    private readonly CarBodyResource _bodyResource;

    private readonly CarWheelResource _wheelResource;

    private readonly float _scale;

    public FourWheeledCarModel(CarBodyResource bodyResource, CarWheelResource wheelResource, float scale)
    {
        _bodyResource = bodyResource;
        _wheelResource = wheelResource;
        _scale = scale;
    }

    public void Render(FourWheeledCar car, Shader shader, float globalScale, float curve, Vector3 cameraPosition, Bodies simulationBodies)
    {
        RenderBody(car.CarBodyPose, shader, globalScale, curve, cameraPosition);
        RenderWheel(GetWheelPosition(car.SimpleCar.FrontLeftWheel.Wheel, simulationBodies), isOnLeftSide: true, shader, globalScale, curve, cameraPosition);
        RenderWheel(GetWheelPosition(car.SimpleCar.FrontRightWheel.Wheel, simulationBodies), isOnLeftSide: false, shader, globalScale, curve, cameraPosition);
        RenderWheel(GetWheelPosition(car.SimpleCar.BackLeftWheel.Wheel, simulationBodies), isOnLeftSide: true, shader, globalScale, curve, cameraPosition);
        RenderWheel(GetWheelPosition(car.SimpleCar.BackRightWheel.Wheel, simulationBodies), isOnLeftSide: false, shader, globalScale, curve, cameraPosition);
    }

    private static RigidPose GetWheelPosition(BodyHandle wheelHandle, Bodies simulationBodies)
    {
        var wheelReference = new BodyReference(wheelHandle, simulationBodies);
        return wheelReference.Pose;
    }

    private void RenderBody(RigidPose bodyPose, Shader shader, float globalScale, float curve, Vector3 cameraPosition)
    {
        if (_bodyResource.Texture == null)
            throw new InvalidOperationException("No texture provided");
        _bodyResource.Texture.Use(TextureUnit.Texture0);

        var translation = Matrix4.CreateTranslation(
        GeomPorting.CreateTranslationTarget(
                Conversions.ToOpenTKVector(bodyPose.Position), cameraPosition, curve, globalScale));

        var rotation = Matrix4.CreateRotationX(-MathF.PI / 2)
            * Conversions.ToOpenTKMatrix(System.Numerics.Matrix4x4.CreateFromQuaternion(bodyPose.Orientation));
        var globalScaleMatrix = Matrix4.CreateScale(globalScale);
        var localScale = Matrix4.CreateScale(_scale);
        shader.SetMatrix4("model", localScale * globalScaleMatrix * rotation * translation);
        shader.SetMatrix4("normalRotation", rotation);

        for (int i = 0; i < _bodyResource.Model.Meshes.Count; i++)
        {
            GL.BindVertexArray(_bodyResource.Vaos[i]);
            GL.DrawElements(PrimitiveType.Triangles, _bodyResource.Model.Meshes[i].FaceCount * 3,
                DrawElementsType.UnsignedInt, 0);
        }
    }

    private void RenderWheel(RigidPose wheelPose, bool isOnLeftSide, Shader shader, float globalScale, float curve, Vector3 cameraPosition)
    {
        if (_wheelResource.Texture == null)
            throw new InvalidOperationException("No texture provided");
        _wheelResource.Texture.Use(TextureUnit.Texture0);

        var translation = Matrix4.CreateTranslation(
       GeomPorting.CreateTranslationTarget(
               Conversions.ToOpenTKVector(wheelPose.Position), cameraPosition, curve, globalScale));

        Matrix4 importRotation;
        if (isOnLeftSide)
        {
            importRotation = Matrix4.CreateRotationZ(-MathF.PI / 2);
        }
        else
        {
            importRotation = Matrix4.CreateRotationZ(MathF.PI / 2);
        }
        var rotation = importRotation
            * Conversions.ToOpenTKMatrix(System.Numerics.Matrix4x4.CreateFromQuaternion(wheelPose.Orientation));

        var globalScaleMatrix = Matrix4.CreateScale(globalScale);
        var localScale = Matrix4.CreateScale(_scale);
        shader.SetMatrix4("model", localScale * globalScaleMatrix * rotation * translation);
        shader.SetMatrix4("normalRotation", rotation);

        for (int i = 0; i < _wheelResource.Model.Meshes.Count; i++)
        {
            GL.BindVertexArray(_wheelResource.Vaos[i]);
            GL.DrawElements(PrimitiveType.Triangles, _wheelResource.Model.Meshes[i].FaceCount * 3,
                DrawElementsType.UnsignedInt, 0);
        }
    }

}
