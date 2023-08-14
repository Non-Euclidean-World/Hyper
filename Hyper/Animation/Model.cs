using BepuPhysics;
using Hyper.TypingUtils;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace Hyper.Animation;

internal class Model
{
    public readonly Animator Animator;

    private readonly int[] _vaos;

    private readonly Texture _texture;

    private readonly Assimp.Scene _model;

    public Model(string modelPath, string texturePath)
    {
        _model = ModelLoader.GetModel(modelPath);
        _vaos = ModelLoader.GetVaos(_model);
        _texture = Texture.LoadFromFile(texturePath);
        Animator = new Animator();
    }

    public void Render(Shader shader, float scale, Vector3 cameraPosition, Vector3 modelPosition, Matrix4 modelRotation)
    {
        _texture.Use(TextureUnit.Texture0);

        var modelLs = Matrix4.CreateTranslation((modelPosition - cameraPosition) * scale);
        var scaleLs = Matrix4.CreateScale(scale);
        shader.SetMatrix4("model", scaleLs * modelRotation * modelLs);

        Animator.Animate(_model);

        for (int i = 0; i < _model.Meshes.Count; i++)
        {
            var boneTransforms = Animator.GetBoneTransforms(_model, i).Select(Conversions.ToOpenTKMatrix).ToArray();
            shader.SetMatrix4Array("boneTransforms", boneTransforms);

            GL.BindVertexArray(_vaos[i]);
            GL.DrawElements(PrimitiveType.Triangles, _model.Meshes[i].FaceCount * 3,
                DrawElementsType.UnsignedInt, 0);
        }
    }

    public void RenderFromPose(RigidPose rigidPose, Shader shader, float globalScale, Vector3 cameraPosition, float localScale, Vector3 localTranslation)
    {
        _texture.Use(TextureUnit.Texture0);

        var localTranslationMatrix = Matrix4.CreateTranslation(localTranslation);
        var translation = Matrix4.CreateTranslation((Conversions.ToOpenTKVector(rigidPose.Position) - cameraPosition) * globalScale);
        var localScaleMatrix = Matrix4.CreateScale(localScale);
        var rotation = Conversions.ToOpenTKMatrix(System.Numerics.Matrix4x4.CreateFromQuaternion(rigidPose.Orientation));
        var globalScaleMatrix = Matrix4.CreateScale(globalScale);

        shader.SetMatrix4("model", localTranslationMatrix * localScaleMatrix * rotation * translation * globalScale);
        shader.SetMatrix4("normalRotation", rotation);

        Animator.Animate(_model);

        for (int i = 0; i < _model.Meshes.Count; i++)
        {
            var boneTransforms = Animator.GetBoneTransforms(_model, i).Select(Conversions.ToOpenTKMatrix).ToArray();
            shader.SetMatrix4Array("boneTransforms", boneTransforms);

            GL.BindVertexArray(_vaos[i]);
            GL.DrawElements(PrimitiveType.Triangles, _model.Meshes[i].FaceCount * 3,
                DrawElementsType.UnsignedInt, 0);
        }
    }

}