using Chunks.ChunkManagement;
using Chunks.Voxels;
using Common.Meshes;
using OpenTK.Mathematics;

namespace Chunks.MarchingCubes.MeshGenerators;

public abstract class BaseMeshGenerator
{
    public const float IsoLevel = 0;

    public abstract Vertex[] GetMesh(Vector3i chunkPosition, ChunkData chunkData);

    protected List<Vertex> GetTriangles(int x, int y, int z, Voxel[,,] scalarField)
    {
        var vertices = new List<Vertex>();

        var (cubeValues, normals, colors) = GetCubeValues(x + 1, y + 1, z + 1, scalarField);
        var edges = GetEdges(cubeValues);
        var position = new Vector3(x, y, z);

        for (int i = 0; edges[i] != -1; i += 3)
        {
            vertices.Add(CreateVertex(i, edges, cubeValues, normals, colors, position));
            vertices.Add(CreateVertex(i + 1, edges, cubeValues, normals, colors, position));
            vertices.Add(CreateVertex(i + 2, edges, cubeValues, normals, colors, position));
        }

        return vertices;
    }

    private Vertex CreateVertex(int index, int[] edges, float[] cubeValues, Vector3[] normals, Vector3[] colors, Vector3 position)
    {
        int edge0 = MarchingCubesTables.EdgeConnections[edges[index]][0];
        int edge1 = MarchingCubesTables.EdgeConnections[edges[index]][1];

        var vertexPosition = Interpolate(MarchingCubesTables.CubeCorners[edge0], cubeValues[edge0], MarchingCubesTables.CubeCorners[edge1], cubeValues[edge1]) + position;
        var normal = Interpolate(normals[edge0], cubeValues[edge0], normals[edge1], cubeValues[edge1]);
        var color = Interpolate(colors[edge0], cubeValues[edge0], colors[edge1], cubeValues[edge1]);

        return new Vertex(vertexPosition, normal, color);
    }

    private static (float[], Vector3[], Vector3[]) GetCubeValues(int x, int y, int z, Voxel[,,] scalarField)
    {
        float[] cubeValues = new float[8];
        Vector3[] colors = new Vector3[8];
        Vector3[] normals = new Vector3[8];
        for (int i = 0; i < 8; i++)
        {
            Vector3i offset = MarchingCubesTables.CubeCorners[i];
            Vector3i cubeVertexPos = new Vector3i(x + offset.X, y + offset.Y, z + offset.Z);
            cubeValues[i] = scalarField[cubeVertexPos.X, cubeVertexPos.Y, cubeVertexPos.Z].Value;
            colors[i] = VoxelHelper.GetColor(scalarField[cubeVertexPos.X, cubeVertexPos.Y, cubeVertexPos.Z].Type);

            normals[i].X = scalarField[cubeVertexPos.X + 1, cubeVertexPos.Y, cubeVertexPos.Z].Value
                           - scalarField[cubeVertexPos.X - 1, cubeVertexPos.Y, cubeVertexPos.Z].Value;
            normals[i].Y = scalarField[cubeVertexPos.X, cubeVertexPos.Y + 1, cubeVertexPos.Z].Value
                           - scalarField[cubeVertexPos.X, cubeVertexPos.Y - 1, cubeVertexPos.Z].Value;
            normals[i].Z = scalarField[cubeVertexPos.X, cubeVertexPos.Y, cubeVertexPos.Z + 1].Value
                           - scalarField[cubeVertexPos.X, cubeVertexPos.Y, cubeVertexPos.Z - 1].Value;
        }

        return (cubeValues, normals, colors);
    }

    private int[] GetEdges(float[] cubeValues)
    {
        int cubeIndex = 0;
        if (cubeValues[0] < IsoLevel) cubeIndex |= 1;
        if (cubeValues[1] < IsoLevel) cubeIndex |= 2;
        if (cubeValues[2] < IsoLevel) cubeIndex |= 4;
        if (cubeValues[3] < IsoLevel) cubeIndex |= 8;
        if (cubeValues[4] < IsoLevel) cubeIndex |= 16;
        if (cubeValues[5] < IsoLevel) cubeIndex |= 32;
        if (cubeValues[6] < IsoLevel) cubeIndex |= 64;
        if (cubeValues[7] < IsoLevel) cubeIndex |= 128;

        return MarchingCubesTables.TriTable[cubeIndex];
    }

    private Vector3 Interpolate(Vector3 edgeVertex1, float valueAtVertex1, Vector3 edgeVertex2, float valueAtVertex2)
    {
        return edgeVertex1 + (IsoLevel - valueAtVertex1) * (edgeVertex2 - edgeVertex1) / (valueAtVertex2 - valueAtVertex1);
    }
}
