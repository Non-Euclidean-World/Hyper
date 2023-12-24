using BepuPhysics;
using Common;
using Common.Utils;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Physics.TypingUtils;

namespace Character.Vehicles;
/// <summary>
/// A model of a car and 4 wheels.
/// </summary>
public class FourWheeledCarModel
{
    private readonly CarBodyResource _bodyResource;

    private readonly CarWheelResource _wheelResource;

    private readonly float _scale;

    /// <summary>
    /// Creates an instance of the <see cref="FourWheeledCarModel"/> class.
    /// </summary>
    /// <param name="bodyResource">The resources of the car model.</param>
    /// <param name="wheelResource">THe resource of the wheels.</param>
    /// <param name="scale">The scale of the car.</param>
    public FourWheeledCarModel(CarBodyResource bodyResource, CarWheelResource wheelResource, float scale)
    {
        _bodyResource = bodyResource;
        _wheelResource = wheelResource;
        _scale = scale;
    }

    /// <summary>
    /// Renders the model of the car.
    /// </summary>
    /// <param name="car">The car.</param>
    /// <param name="shader">The shader used for rendering.</param>
    /// <param name="globalScale">The scale of the scene.</param>
    /// <param name="curve">The curvature of the scene.</param>
    /// <param name="cameraPosition">The position of the camera in the scene.</param>
    /// <param name="simulationBodies">The bodies in the simulation.</param>
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

        var translation = Matrices.TranslationMatrix(GeomPorting.EucToCurved(GeomPorting.CreateTranslationTarget(Conversions.ToOpenTKVector(bodyPose.Position), cameraPosition, curve, globalScale), curve), curve);

        var rotation = Matrix4.CreateRotationX(-MathF.PI / 2)
            * Conversions.ToOpenTKMatrix(System.Numerics.Matrix4x4.CreateFromQuaternion(bodyPose.Orientation));
        var globalScaleMatrix = Matrix4.CreateScale(globalScale);
        var localScale = Matrix4.CreateScale(_scale);
        shader.SetMatrix4("model", localScale * globalScaleMatrix);
        shader.SetMatrix4("rotation", rotation);
        shader.SetMatrix4("translation", translation);
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

        var translation = Matrices.TranslationMatrix(GeomPorting.EucToCurved(GeomPorting.CreateTranslationTarget(Conversions.ToOpenTKVector(wheelPose.Position), cameraPosition, curve, globalScale), curve), curve);

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
        shader.SetMatrix4("model", localScale * globalScaleMatrix);
        shader.SetMatrix4("translation", translation);
        shader.SetMatrix4("rotation", rotation);
        shader.SetMatrix4("normalRotation", rotation);

        for (int i = 0; i < _wheelResource.Model.Meshes.Count; i++)
        {
            GL.BindVertexArray(_wheelResource.Vaos[i]);
            GL.DrawElements(PrimitiveType.Triangles, _wheelResource.Model.Meshes[i].FaceCount * 3,
                DrawElementsType.UnsignedInt, 0);
        }
    }

}
