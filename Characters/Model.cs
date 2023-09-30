using BepuPhysics;
using Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Physics.TypingUtils;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace Character;

public abstract class Model
{
    public readonly Animator Animator;

    private readonly ModelResources _modelResources;

    private readonly float _localScale;

    private readonly Vector3 _localTranslation;

    public Model(ModelResources modelResources, float localScale, Vector3 localTranslation)
    {
        _modelResources = modelResources;
        Animator = new Animator();
        _localScale = localScale;
        _localTranslation = localTranslation;
    }

    public void Render(RigidPose rigidPose, Shader shader, float globalScale, Vector3 cameraPosition)
    {
        _modelResources.Texture.Use(TextureUnit.Texture0);

        var localTranslationMatrix = Matrix4.CreateTranslation(_localTranslation);
        var translation = Matrix4.CreateTranslation((Conversions.ToOpenTKVector(rigidPose.Position) - cameraPosition) * globalScale);
        var localScaleMatrix = Matrix4.CreateScale(_localScale);
        var rotation = Conversions.ToOpenTKMatrix(System.Numerics.Matrix4x4.CreateFromQuaternion(rigidPose.Orientation));

        shader.SetMatrix4("model", localTranslationMatrix * localScaleMatrix * rotation * translation * globalScale);
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