using Assimp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace Hyper.Animation;

internal class Model
{
    private readonly int _vao;
    private readonly Texture _texture;
    private readonly Assimp.Scene _model;
    
    public Model()
    {
        var path = "Animation/Resources/model.dae";
        _model = ModelLoader.GetModel(path);
        _vao = ModelLoader.GetVao(_model);
        _texture = Texture.LoadFromFile("Animation/Resources/diffuse.png");
    }

    public void Render(Shader shader, float scale, Vector3 cameraPosition)
    {
        _texture.Use(TextureUnit.Texture0);
        
        var modelLs = Matrix4.CreateTranslation((new Vector3(0, 20, 0) - cameraPosition) * scale);
        var scaleLs = Matrix4.CreateScale(scale);
        shader.SetMatrix4("model", scaleLs * modelLs);
        
        var boneTransforms = GetBoneTransforms();
        for (int i = 0; i < _model.Meshes[0].BoneCount; i++)
        {
            shader.SetMatrix4($"jointTransforms[{i}]", boneTransforms[i]);
        }
        
        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, _model.Meshes[0].FaceCount * 3,
            DrawElementsType.UnsignedInt, 0);
    }
    
    private Matrix4[] GetBoneTransforms() => 
        _model.Meshes[0].Bones
        .Select(bone => AssimpToOpenTk(bone.OffsetMatrix))
        .ToArray();
    
    private static Matrix4 AssimpToOpenTk(Matrix4x4 assimpMatrix)
    {
        return new Matrix4(
            assimpMatrix.A1, assimpMatrix.A2, assimpMatrix.A3, assimpMatrix.A4,
            assimpMatrix.B1, assimpMatrix.B2, assimpMatrix.B3, assimpMatrix.B4,
            assimpMatrix.C1, assimpMatrix.C2, assimpMatrix.C3, assimpMatrix.C4,
            assimpMatrix.D1, assimpMatrix.D2, assimpMatrix.D3, assimpMatrix.D4);
    }
}