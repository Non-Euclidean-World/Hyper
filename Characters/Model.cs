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

    private readonly int[] _vaos;

    private readonly Texture _texture;

    private readonly Assimp.Scene _model;

    private readonly float _localScale;

    private readonly Vector3 _localTranslation;

    public Model(string modelPath, string texturePath, float localScale, Vector3 localTranslation)
    {
        _model = ModelLoader.GetModel(modelPath);
        _vaos = ModelLoader.GetVaos(_model);
        _texture = Texture.LoadFromFile(texturePath);
        Animator = new Animator();
        _localScale = localScale;
        _localTranslation = localTranslation;
    }

    public void Render(RigidPose rigidPose, Shader shader, float globalScale, float curve, Vector3 cameraPosition)
    {
        _texture.Use(TextureUnit.Texture0);

        var localTranslationMatrix = Matrix4.CreateTranslation(_localTranslation);
        var translation = Matrix4.CreateTranslation(
            GeomPorting.CreateTranslationTarget(Conversions.ToOpenTKVector(rigidPose.Position), cameraPosition, curve, globalScale));
        var localScaleMatrix = Matrix4.CreateScale(_localScale);
        var rotation = Conversions.ToOpenTKMatrix(System.Numerics.Matrix4x4.CreateFromQuaternion(rigidPose.Orientation));
        var globalScaleMatrix = Matrix4.CreateScale(globalScale);

        shader.SetMatrix4("model", localTranslationMatrix * localScaleMatrix * globalScaleMatrix * rotation * translation);
        shader.SetMatrix4("normalRotation", rotation);

        Animator.Animate(_model);

        for (int i = 0; i < _model.Meshes.Count; i++)
        {
            var boneTransforms = Animator.GetBoneTransforms(_model, i).Select(AssimpConversions.ToOpenTKMatrix).ToArray();
            shader.SetMatrix4Array("boneTransforms", boneTransforms);

            GL.BindVertexArray(_vaos[i]);
            GL.DrawElements(PrimitiveType.Triangles, _model.Meshes[i].FaceCount * 3,
                DrawElementsType.UnsignedInt, 0);
        }
    }

}