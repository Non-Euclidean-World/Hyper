using Common;
using Common.ResourceClasses;
using Common.UserInput;
using Common.Utils;
using Hyper.Shaders.LightSourceShader;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Physics.BoundingShapes;
using Physics.TypingUtils;

namespace Hyper.Controllers;
internal class BoundingShapesController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly ShapesExtractor _shapesExtractor;

    private readonly AbstractLightSourceShader _shader;

    private readonly BoxResource _boxResource = BoxResource.Instance;

    private readonly CapsuleResource _capsuleResource = CapsuleResource.Instance;

    private readonly CylinderResource _cylinderResource = CylinderResource.Instance;

    private bool _active = false;

    public BoundingShapesController(Scene scene, AbstractLightSourceShader shader, Context context)
    {
        _scene = scene;
        _shader = shader;
        _shapesExtractor = new ShapesExtractor(_scene.SimulationManager.Simulation);
        RegisterCallbacks(context);
    }

    public void Render()
    {
        if (!_active)
            return;
        _shapesExtractor.ClearCache();
        _shapesExtractor.AddShapes(_scene.SimulationMembers);

        TurnOnWireframe();

        _shader.SetUp(_scene.Camera);
        for (int i = 0; i < _shapesExtractor.Boxes.Count; i++)
        {
            ref var box = ref _shapesExtractor.Boxes[i];
            _shader.SetInt("sphere", box.SphereId);
            _shader.SetVector3("color", Conversions.ToOpenTKVector(box.Color));
            Render(box);
        }
        for (int i = 0; i < _shapesExtractor.Capsules.Count; i++)
        {
            ref var capsule = ref _shapesExtractor.Capsules[i];
            _shader.SetInt("sphere", capsule.SphereId);
            _shader.SetVector3("color", Conversions.ToOpenTKVector(capsule.Color));
            Render(capsule);
        }

        for (int i = 0; i < _shapesExtractor.Cylinders.Count; i++)
        {
            ref var cylinder = ref _shapesExtractor.Cylinders[i];
            _shader.SetInt("sphere", cylinder.SphereId);
            _shader.SetVector3("color", Conversions.ToOpenTKVector(cylinder.Color));
            Render(cylinder);
        }

        TurnOffWireframe();
    }
    private void Render(BoundingBox box)
    {
        var translation = Matrices.TranslationMatrix(GeomPorting.EucToCurved(GeomPorting.CreateTranslationTarget(
                Conversions.ToOpenTKVector(box.Pose.Position), _scene.Camera.ReferencePointPosition, _scene.Camera.Curve, _scene.GlobalScale), _scene.Camera.Curve), _scene.Camera.Curve);

        var rotation = Conversions.ToOpenTKMatrix(System.Numerics.Matrix4x4.CreateFromQuaternion(box.Pose.Orientation));
        var globalScale = Matrix4.CreateScale(_scene.GlobalScale);
        var localScale = Matrix4.CreateScale(new Vector3(box.HalfWidth, box.HalfHeight, box.HalfLength));
        _shader.SetMatrix4("model", localScale * globalScale);
        _shader.SetMatrix4("rotation", rotation);
        _shader.SetMatrix4("translation", translation);

        GL.BindVertexArray(_boxResource.Vaos[0]);
        GL.DrawElements(PrimitiveType.Triangles, _boxResource.Model.Meshes[0].FaceCount * 3,
            DrawElementsType.UnsignedInt, 0);
    }

    private void Render(BoundingCapsule capsule)
    {
        var translation = Matrices.TranslationMatrix(GeomPorting.EucToCurved(GeomPorting.CreateTranslationTarget(
        Conversions.ToOpenTKVector(capsule.Pose.Position), _scene.Camera.ReferencePointPosition, _scene.Camera.Curve, _scene.GlobalScale), _scene.Camera.Curve), _scene.Camera.Curve);

        var rotation = Conversions.ToOpenTKMatrix(System.Numerics.Matrix4x4.CreateFromQuaternion(capsule.Pose.Orientation));
        var globalScale = Matrix4.CreateScale(_scene.GlobalScale);
        var localScale = Matrix4.CreateScale(new Vector3(capsule.Radius, capsule.HalfLength, capsule.Radius));
        _shader.SetMatrix4("model", localScale * globalScale);
        _shader.SetMatrix4("rotation", rotation);
        _shader.SetMatrix4("translation", translation);

        GL.BindVertexArray(_capsuleResource.Vaos[0]);
        GL.DrawElements(PrimitiveType.Triangles, _capsuleResource.Model.Meshes[0].FaceCount * 3,
            DrawElementsType.UnsignedInt, 0);
    }

    private void Render(BoundingCylinder cylinder)
    {
        var translation = Matrices.TranslationMatrix(GeomPorting.EucToCurved(GeomPorting.CreateTranslationTarget(
                Conversions.ToOpenTKVector(cylinder.Pose.Position), _scene.Camera.ReferencePointPosition, _scene.Camera.Curve, _scene.GlobalScale), _scene.Camera.Curve), _scene.Camera.Curve);

        var rotation = Conversions.ToOpenTKMatrix(System.Numerics.Matrix4x4.CreateFromQuaternion(cylinder.Pose.Orientation));
        var globalScale = Matrix4.CreateScale(_scene.GlobalScale);
        var localScale = Matrix4.CreateScale(new Vector3(cylinder.Radius, cylinder.HalfLength, cylinder.Radius));
        _shader.SetMatrix4("model", localScale * globalScale);
        _shader.SetMatrix4("rotation", rotation);
        _shader.SetMatrix4("translation", translation);

        GL.BindVertexArray(_cylinderResource.Vaos[0]);
        GL.DrawElements(PrimitiveType.Triangles, _cylinderResource.Model.Meshes[0].FaceCount * 3,
            DrawElementsType.UnsignedInt, 0);
    }

    private static void TurnOnWireframe()
    {
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
    }

    private static void TurnOffWireframe()
    {
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
    }

    public void Dispose()
    {
        _shapesExtractor.Dispose();
    }

    public void RegisterCallbacks(Context context)
    {
        context.RegisterKeys(new List<Keys> { Keys.F3 });
        context.RegisterKeyDownCallback(Keys.F3, () => _active = !_active);
    }
}
