using Hyper.MarchingCubes.Voxels;
using Hyper.Meshes;
using OpenTK.Mathematics;

namespace Hyper.MarchingCubes;

internal class MeshGenerator
{
    private readonly float _isolevel;
    private readonly Voxel[,,] _scalarField;

    public MeshGenerator(Voxel[,,] scalarField, float isolevel = 0f)
    {
        _scalarField = scalarField;
        _isolevel = isolevel;
    }

    public Vertex[] GetMesh()
    {
        var vertices = new List<Vertex>();
        for (int x = 0; x < _scalarField.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < _scalarField.GetLength(1) - 1; y++)
            {
                for (int z = 0; z < _scalarField.GetLength(2) - 1; z++)
                {
                    vertices.AddRange(GetTriangles(x, y, z));
                }
            }
        }

        return vertices.ToArray();
    }

    private List<Vertex> GetTriangles(int x, int y, int z)
    {
        var vertices = new List<Vertex>();

        var (cubeValues, normals, colors) = GetCubeValues(x, y, z);
        var edges = GetEdges(cubeValues);
        var position = new Vector3(x, y, z);

        for (int i = 0; edges[i] != -1; i += 3)
        {
            int e00 = MarchingCubesTables.EdgeConnections[edges[i]][0];
            int e01 = MarchingCubesTables.EdgeConnections[edges[i]][1];

            int e10 = MarchingCubesTables.EdgeConnections[edges[i + 1]][0];
            int e11 = MarchingCubesTables.EdgeConnections[edges[i + 1]][1];

            int e20 = MarchingCubesTables.EdgeConnections[edges[i + 2]][0];
            int e21 = MarchingCubesTables.EdgeConnections[edges[i + 2]][1];

            var a = Interpolate(MarchingCubesTables.CubeCorners[e00], cubeValues[e00], MarchingCubesTables.CubeCorners[e01], cubeValues[e01]) + position;
            var b = Interpolate(MarchingCubesTables.CubeCorners[e10], cubeValues[e10], MarchingCubesTables.CubeCorners[e11], cubeValues[e11]) + position;
            var c = Interpolate(MarchingCubesTables.CubeCorners[e20], cubeValues[e20], MarchingCubesTables.CubeCorners[e21], cubeValues[e21]) + position;

            var na = Interpolate(normals[e00], cubeValues[e00], normals[e01], cubeValues[e01]);
            var nb = Interpolate(normals[e10], cubeValues[e10], normals[e11], cubeValues[e11]);
            var nc = Interpolate(normals[e20], cubeValues[e20], normals[e21], cubeValues[e21]);

            var colorA = Interpolate(colors[e00], cubeValues[e00], colors[e01], cubeValues[e01]);
            var colorB = Interpolate(colors[e10], cubeValues[e10], colors[e11], cubeValues[e11]);
            var colorC = Interpolate(colors[e20], cubeValues[e20], colors[e21], cubeValues[e21]);

            vertices.Add(new Vertex(a, na, colorA));
            vertices.Add(new Vertex(b, nb, colorB));
            vertices.Add(new Vertex(c, nc, colorC));
        }

        return vertices;
    }

    private (float[], Vector3[], Vector3[]) GetCubeValues(int x, int y, int z)
    {
        float[] cubeValues = new float[8];
        Vector3[] colors = new Vector3[8];
        Vector3[] normals = new Vector3[8];
        for (int i = 0; i < 8; i++)
        {
            Vector3i offset = MarchingCubesTables.CubeCorners[i];
            Vector3i cubeVertexPos = new Vector3i(x + offset.X, y + offset.Y, z + offset.Z);
            cubeValues[i] = _scalarField[cubeVertexPos.X, cubeVertexPos.Y, cubeVertexPos.Z].Value;
            colors[i] = VoxelHelper.GetColor(_scalarField[cubeVertexPos.X, cubeVertexPos.Y, cubeVertexPos.Z].Type);

            if (cubeVertexPos.X == 0 || cubeVertexPos.Y == 0 || cubeVertexPos.Z == 0
                || cubeVertexPos.X == _scalarField.GetLength(0) - 1 || cubeVertexPos.Y == _scalarField.GetLength(1) - 1 || cubeVertexPos.Z == _scalarField.GetLength(2) - 1)
            {
                normals[i] = new Vector3(1, 1, 1); // TODO fix this
                continue;
            }

            normals[i].X = _scalarField[cubeVertexPos.X + 1, cubeVertexPos.Y, cubeVertexPos.Z].Value - _scalarField[cubeVertexPos.X - 1, cubeVertexPos.Y, cubeVertexPos.Z].Value;
            normals[i].Y = _scalarField[cubeVertexPos.X, cubeVertexPos.Y + 1, cubeVertexPos.Z].Value - _scalarField[cubeVertexPos.X, cubeVertexPos.Y - 1, cubeVertexPos.Z].Value;
            normals[i].Z = _scalarField[cubeVertexPos.X, cubeVertexPos.Y, cubeVertexPos.Z + 1].Value - _scalarField[cubeVertexPos.X, cubeVertexPos.Y, cubeVertexPos.Z - 1].Value;
        }

        return (cubeValues, normals, colors);
    }

    private int[] GetEdges(float[] cubeValues)
    {
        int cubeIndex = 0;
        if (cubeValues[0] < _isolevel) cubeIndex |= 1;
        if (cubeValues[1] < _isolevel) cubeIndex |= 2;
        if (cubeValues[2] < _isolevel) cubeIndex |= 4;
        if (cubeValues[3] < _isolevel) cubeIndex |= 8;
        if (cubeValues[4] < _isolevel) cubeIndex |= 16;
        if (cubeValues[5] < _isolevel) cubeIndex |= 32;
        if (cubeValues[6] < _isolevel) cubeIndex |= 64;
        if (cubeValues[7] < _isolevel) cubeIndex |= 128;

        return MarchingCubesTables.TriTable[cubeIndex];
    }

    private Vector3 Interpolate(Vector3 edgeVertex1, float valueAtVertex1, Vector3 edgeVertex2, float valueAtVertex2)
    {
        return edgeVertex1 + (_isolevel - valueAtVertex1) * (edgeVertex2 - edgeVertex1) / (valueAtVertex2 - valueAtVertex1);
    }
}
