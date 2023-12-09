using BepuPhysics;
using Common;
using Common.ResourceClasses;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Physics.TypingUtils;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace Character;
/// <summary>
/// Represents a 3D model.
/// </summary>
public class Model
{
    /// <summary>
    /// Is responsible for animating the model.
    /// </summary>
    public readonly Animator Animator;

    private readonly ModelResource _modelResources;

    private readonly float _localScale;

    private readonly Vector3 _localTranslation;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Model"/> class.
    /// </summary>
    /// <param name="modelResources">The resources for the model.</param>
    /// <param name="localScale">The scale of the model.</param>
    /// <param name="localTranslation">The transformation for the model.</param>
    public Model(ModelResource modelResources, float localScale, Vector3 localTranslation)
    {
        _modelResources = modelResources;
        Animator = new Animator();
        _localScale = localScale;
        _localTranslation = localTranslation;
    }

    /// <summary>
    /// Renders the model.
    /// </summary>
    /// <param name="rigidPose">The pose of the model.</param>
    /// <param name="shader">The shader used for rendering.</param>
    /// <param name="globalScale">The global scale of the scene.</param>
    /// <param name="curve">The curvature of the scene.</param>
    /// <param name="cameraPosition">The position of the camera in the scene.</param>
    /// <exception cref="InvalidOperationException">Some resource was not provided.</exception>
    public void Render(RigidPose rigidPose, Shader shader, float globalScale, float curve, Vector3 cameraPosition)
    {
        if (_modelResources.Texture == null)
            throw new InvalidOperationException("No texture provided");
        _modelResources.Texture.Use(TextureUnit.Texture0);

        var localTranslationMatrix = Matrix4.CreateTranslation(_localTranslation);
        var translation = Matrix4.CreateTranslation(
            GeomPorting.CreateTranslationTarget(Conversions.ToOpenTKVector(rigidPose.Position), cameraPosition, curve, globalScale));
        var localScaleMatrix = Matrix4.CreateScale(_localScale);
        var rotation = Conversions.ToOpenTKMatrix(System.Numerics.Matrix4x4.CreateFromQuaternion(rigidPose.Orientation));
        var globalScaleMatrix = Matrix4.CreateScale(globalScale);

        shader.SetMatrix4("model", localTranslationMatrix * localScaleMatrix * globalScaleMatrix * rotation * translation);
        shader.SetMatrix4("normalRotation", rotation);

        Animator.Animate(_modelResources.Model);

        for (int i = 0; i < _modelResources.Model.Meshes.Count; i++)
        {
            var boneTransforms = Animator.GetBoneTransforms(_modelResources.Model, i).Select(AssimpConversions.ToOpenTKMatrix).ToArray();
            shader.SetMatrix4Array("boneTransforms", boneTransforms);

            GL.BindVertexArray(_modelResources.Vaos[i]);
            GL.DrawElements(PrimitiveType.Triangles, _modelResources.Model.Meshes[i].FaceCount * 3,
                DrawElementsType.UnsignedInt, 0);
        }
    }
}