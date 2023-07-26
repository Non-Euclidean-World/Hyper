using Assimp;
using OpenTK.Graphics.OpenGL4;

namespace Hyper.Animation;

public static class ModelLoader
{
    public static Assimp.Scene GetModel(string path)
    {
        AssimpContext importer = new AssimpContext();
        return importer.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);
    }

    public static int GetVao(Assimp.Scene model)
    {
        int vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);

        foreach (var mesh in model.Meshes)
        {
            GetPositions(mesh);
            GetNormals(mesh);
            GetTextureCoords(mesh);
            GetBones(mesh);
            GetFaces(mesh);
        }
        
        GL.BindVertexArray(0);

        return vao;
    }

    private static void GetPositions(Assimp.Mesh mesh)
    {
        int vboPositions = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vboPositions);
            
        var vertices = mesh.Vertices.SelectMany(v => new[] { v.X, v.Y, v.Z }).ToArray();
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(0);
    }

    private static void GetNormals(Assimp.Mesh mesh)
    {
        int vboNormals = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vboNormals);

        var normals = mesh.Normals.SelectMany(n => new[] { n.X, n.Y, n.Z }).ToArray();
        GL.BufferData(BufferTarget.ArrayBuffer, normals.Length * sizeof(float), normals, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(1);
    }
    
    private static void GetTextureCoords(Assimp.Mesh mesh)
    {
        if (!mesh.HasTextureCoords(0)) return;
        
        int vboTexCoords = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vboTexCoords);

        var texCoords = mesh.TextureCoordinateChannels[0].SelectMany(t => new[] { t.X, t.Y }).ToArray();
        GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Length * sizeof(float), texCoords, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(2);
    }

    private static void GetBones(Assimp.Mesh mesh)
    {
        List<int> boneIndices = new List<int>();
        List<float> boneWeights = new List<float>();

        for (int boneIndex = 0; boneIndex < mesh.BoneCount; boneIndex++)
        {
            var bone = mesh.Bones[boneIndex];

            // Go through each vertex weight in the bone
            foreach (var weight in bone.VertexWeights)
            {
                // The vertex ID is the index of the vertex this weight applies to
                int vertexId = weight.VertexID;

                // Store the bone index and weight
                boneIndices.Add(boneIndex);
                boneWeights.Add(weight.Weight);
            }
        }

        int vboBoneIndices = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vboBoneIndices);
        GL.BufferData(BufferTarget.ArrayBuffer, boneIndices.Count * sizeof(int), boneIndices.ToArray(), BufferUsageHint.StaticDraw);

        int vboBoneWeights = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vboBoneWeights);
        GL.BufferData(BufferTarget.ArrayBuffer, boneWeights.Count * sizeof(float), boneWeights.ToArray(), BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Int, false, 0, 0);
        GL.EnableVertexAttribArray(3);

        GL.VertexAttribPointer(4, 4, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(4);
    }
    
    private static void GetFaces(Assimp.Mesh mesh)
    {
        if (!mesh.HasFaces) return;
        
        int ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

        // Get the face indices and pass them to the GPU
        var indices = mesh.Faces.SelectMany(f => f.Indices).ToArray();
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);
    }
}