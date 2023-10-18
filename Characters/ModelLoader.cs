using Assimp;
using OpenTK.Graphics.OpenGL4;

namespace Character;

public static class ModelLoader
{
    public static Scene GetModel(string path)
    {
        AssimpContext importer = new AssimpContext();
        return importer.ImportFile(path);
    }

    public static int[] GetVaos(Scene model)
    {
        var vaos = new List<int>();

        foreach (var mesh in model.Meshes)
        {
            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            SetupPositions(mesh);
            SetupNormals(mesh);
            SetupTextureCoords(mesh);
            SetupBones(mesh);
            SetupFaces(mesh);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            vaos.Add(vao);
        }

        return vaos.ToArray();
    }

    private static void SetupPositions(Mesh mesh)
    {
        int vboPositions = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vboPositions);

        var vertices = mesh.Vertices.SelectMany(v => new[] { v.X, v.Y, v.Z }).ToArray();
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(0);
    }

    private static void SetupNormals(Mesh mesh)
    {
        int vboNormals = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vboNormals);

        var normals = mesh.Normals.SelectMany(n => new[] { n.X, n.Y, n.Z }).ToArray();
        GL.BufferData(BufferTarget.ArrayBuffer, normals.Length * sizeof(float), normals, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(1);
    }

    private static void SetupTextureCoords(Mesh mesh)
    {
        if (!mesh.HasTextureCoords(0)) return;

        int vboTexCoords = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vboTexCoords);

        var texCoords = mesh.TextureCoordinateChannels[0].SelectMany(t => new[] { t.X, t.Y }).ToArray();
        GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Length * sizeof(float), texCoords, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(2);
    }

    private static void SetupBones(Mesh mesh)
    {
        if (!mesh.HasBones)
            return;

        const int maxBones = 3;

        int[,] vertexBones = new int[mesh.VertexCount, maxBones];
        float[,] vertexWeights = new float[mesh.VertexCount, maxBones];

        for (int boneIndex = 0; boneIndex < mesh.BoneCount; boneIndex++)
        {
            var bone = mesh.Bones[boneIndex];

            foreach (var weight in bone.VertexWeights)
            {
                for (int slot = 0; slot < maxBones; slot++)
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

        int[] boneIndices = new int[mesh.VertexCount * maxBones];
        System.Buffer.BlockCopy(vertexBones, 0, boneIndices, 0, boneIndices.Length * sizeof(int));

        float[] boneWeights = new float[mesh.VertexCount * maxBones];
        System.Buffer.BlockCopy(vertexWeights, 0, boneWeights, 0, boneWeights.Length * sizeof(float));

        int vboBoneIndices = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vboBoneIndices);
        GL.BufferData(BufferTarget.ArrayBuffer, boneIndices.Length * sizeof(int), boneIndices.ToArray(), BufferUsageHint.StaticDraw);

        GL.VertexAttribIPointer(3, maxBones, VertexAttribIntegerType.Int, 0, 0);
        GL.EnableVertexAttribArray(3);

        int vboBoneWeights = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vboBoneWeights);
        GL.BufferData(BufferTarget.ArrayBuffer, boneWeights.Length * sizeof(float), boneWeights.ToArray(), BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(4, maxBones, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(4);
    }

    private static void SetupFaces(Mesh mesh)
    {
        if (!mesh.HasFaces) return;

        int ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

        var indices = mesh.Faces.SelectMany(f => f.Indices).ToArray();
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);
    }
}