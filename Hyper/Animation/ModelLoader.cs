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
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
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
        // Initialize arrays of bones and weights for each vertex in the mesh
        int[,] vertexBones = new int[mesh.VertexCount, 3];
        float[,] vertexWeights = new float[mesh.VertexCount, 3];

        // Go through each bone in the mesh
        for (int boneIndex = 0; boneIndex < mesh.BoneCount; boneIndex++)
        {
            var bone = mesh.Bones[boneIndex];

            // Go through each vertex weight in the bone
            foreach (var weight in bone.VertexWeights)
            {
                // Find the next available slot for this vertex
                for (int slot = 0; slot < 3; slot++)
                {
                    if (vertexBones[weight.VertexID, slot] == 0 && vertexWeights[weight.VertexID, slot] == 0f)
                    {
                        vertexBones[weight.VertexID, slot] = boneIndex;
                        vertexWeights[weight.VertexID, slot] = weight.Weight;
                        break;
                    }
                }
            }
        }
        
        int[] boneIndices = new int[mesh.VertexCount * 3];
        for (int i = 0; i < mesh.VertexCount; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                boneIndices[i * 3 + j] = vertexBones[i, j];
            }
        }

        float[] boneWeights = new float[mesh.VertexCount * 3];
        for (int i = 0; i < mesh.VertexCount; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                boneWeights[i * 3 + j] = vertexWeights[i, j];
            }
        }

        int size = 3;

        int vboBoneIndices = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vboBoneIndices);
        GL.BufferData(BufferTarget.ArrayBuffer, boneIndices.Length * sizeof(int), boneIndices.ToArray(), BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(3, size, VertexAttribPointerType.Int, false, 0, 0);
        GL.EnableVertexAttribArray(3);
        
        int vboBoneWeights = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vboBoneWeights);
        GL.BufferData(BufferTarget.ArrayBuffer, boneWeights.Length * sizeof(float), boneWeights.ToArray(), BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(4, size, VertexAttribPointerType.Float, false, 0, 0);
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