using Assimp;
using BepuPhysics;
using Hyper.Collisions;
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
            var boneTransforms = Animator.GetBoneTransforms(_model, i).Select(AssimpToOpenTk).ToArray();
            shader.SetMatrix4Array("boneTransforms", boneTransforms);

            GL.BindVertexArray(_vaos[i]);
            GL.DrawElements(PrimitiveType.Triangles, _model.Meshes[i].FaceCount * 3,
                DrawElementsType.UnsignedInt, 0);
        }
    }

    public void RenderFromPose(RigidPose rigidPose, Shader shader, float scale, Vector3 cameraPosition)
    {
        _texture.Use(TextureUnit.Texture0);

        var translation = Matrix4.CreateTranslation((TypingUtils.ToOpenTKVector(rigidPose.Position) - cameraPosition) * scale);
        var scaleMatrix = Matrix4.CreateScale(scale);
        var rotation = TypingUtils.ToOpenTKMatrix(System.Numerics.Matrix4x4.CreateFromQuaternion(rigidPose.Orientation));

        shader.SetMatrix4("model", scaleMatrix * rotation * translation);

        Animator.Animate(_model);

        for (int i = 0; i < _model.Meshes.Count; i++)
        {
            var boneTransforms = Animator.GetBoneTransforms(_model, i).Select(AssimpToOpenTk).ToArray();
            shader.SetMatrix4Array("boneTransforms", boneTransforms);

            GL.BindVertexArray(_vaos[i]);
            GL.DrawElements(PrimitiveType.Triangles, _model.Meshes[i].FaceCount * 3,
                DrawElementsType.UnsignedInt, 0);
        }
    }

    private static Matrix4 AssimpToOpenTk(Matrix4x4 assimpMatrix)
    {
        return new Matrix4(
            assimpMatrix.A1, assimpMatrix.A2, assimpMatrix.A3, assimpMatrix.A4,
            assimpMatrix.B1, assimpMatrix.B2, assimpMatrix.B3, assimpMatrix.B4,
            assimpMatrix.C1, assimpMatrix.C2, assimpMatrix.C3, assimpMatrix.C4,
            assimpMatrix.D1, assimpMatrix.D2, assimpMatrix.D3, assimpMatrix.D4);
    }
}