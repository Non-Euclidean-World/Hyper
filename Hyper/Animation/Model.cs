using Assimp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace Hyper.Animation;

internal class Model
{
    private readonly int[] _vaos;
    private readonly Texture _texture;
    private readonly Assimp.Scene _model;
    private readonly Animator _animator;
    
    public Model()
    {
        var path = "Animation/Resources/model.dae";
        _model = ModelLoader.GetModel(path);
        _vaos = ModelLoader.GetVao(_model);
        _texture = Texture.LoadFromFile("Animation/Resources/diffuse.png");

        _animator = new Animator();
    }

    public void Render(Shader shader, float scale, Vector3 cameraPosition)
    {
        _texture.Use(TextureUnit.Texture0);
        
        var modelLs = Matrix4.CreateTranslation((new Vector3(0, 20, 0) - cameraPosition) * scale);
        var scaleLs = Matrix4.CreateScale(scale);
        shader.SetMatrix4("model", scaleLs * modelLs);
        
        _animator.Animate(_model);
        
        for (int i = 0; i < _model.Meshes.Count; i++)
        {
            var boneTransforms = _animator.GetBones(_model, i).Select(bone => AssimpToOpenTk(bone)).ToArray();
            shader.SetMatrix4Array("jointTransforms", boneTransforms);
            
            GL.BindVertexArray(_vaos[i]);
            GL.DrawElements(PrimitiveType.Triangles, _model.Meshes[i].FaceCount * 3,
                DrawElementsType.UnsignedInt, 0);
        }
    }
    
    private Matrix4[] GetBoneTransforms(int i) => 
        _model.Meshes[i].Bones
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