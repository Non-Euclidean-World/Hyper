using Chunks.MarchingCubes;
using Chunks.Voxels;
using Common.Meshes;
using OpenTK.Mathematics;

namespace Chunks;
public class SphericalChunkFactory
{
    private readonly ScalarFieldGenerator _scalarFieldGenerator;

    private readonly Vector3i[] _sphereCenters;

    private readonly float _globalScale;

    public SphericalChunkFactory(ScalarFieldGenerator scalarFieldGenerator, Vector3i[] sphereCenters, float globalScale)
    {
        _scalarFieldGenerator = scalarFieldGenerator;
        _sphereCenters = sphereCenters;
        _globalScale = globalScale;
    }

    public List<Chunk> CreateSpheres(int chunksPerSide, bool generateVao = true)
    {
        if (chunksPerSide % 2 != 0)
            throw new ArgumentException("# of chunks/side must be even");

        List<Chunk> spheres = new();

        for (int chunkX = -chunksPerSide / 2; chunkX < chunksPerSide / 2; chunkX++)
        {
            for (int chunkY = -chunksPerSide / 2; chunkY < chunksPerSide / 2; chunkY++)
            {
                Voxel[,,] averageScalarField0 = new Voxel[Chunk.Size, Chunk.Size, Chunk.Size];
                Voxel[,,] averageScalarField1 = new Voxel[Chunk.Size, Chunk.Size, Chunk.Size];

                int offset = Chunk.Size - 1;

                var sf0Pos = new Vector3i(offset * chunkX, 0, offset * chunkY) + _sphereCenters[0];
                var sf1Pos = new Vector3i(offset * chunkX, 0, offset * chunkY) + _sphereCenters[1];

                var scalarField0 = _scalarFieldGenerator.Generate(Chunk.Size, sf0Pos);
                var scalarField1 = _scalarFieldGenerator.Generate(Chunk.Size, sf1Pos);

                for (int x = 0; x < Chunk.Size; x++)
                {
                    for (int y = 0; y < Chunk.Size; y++)
                    {
                        for (int z = 0; z < Chunk.Size; z++)
                        {
                            Vector3i position = new Vector3i(x + chunkX * Chunk.Size, y, z + chunkY * Chunk.Size);
                            float radius = MathF.PI / 2 / _globalScale;
                            float boundaryValue = GetBoundaryValue(position);

                            float d = Vector3.Distance(position, _sphereCenters[0]);
                            averageScalarField0[x, y, z].Value = F1(d, radius) * boundaryValue + F2(d, radius) * scalarField0[x, y, z].Value;
                            averageScalarField1[x, y, z].Value = F1(d, radius) * boundaryValue + F2(d, radius) * scalarField1[x, y, z].Value;

                            VoxelType type;
                            if (y < Chunk.Size / 2.5) type = VoxelType.Rock;
                            else if (y < Chunk.Size / 2) type = VoxelType.GrassRock;
                            else type = VoxelType.Grass;

                            averageScalarField0[x, y, z].Type = averageScalarField1[x, y, z].Type = type;
                        }
                    }
                }
                var meshGenerator0 = new MeshGenerator(averageScalarField0);
                var meshGenerator1 = new MeshGenerator(averageScalarField1);
                var sfPos0 = new Vector3i(offset * chunkX, 0, offset * chunkY) + _sphereCenters[0] - new Vector3i(0, (int)_scalarFieldGenerator.AvgElevation, 0);
                Vertex[] data0 = meshGenerator0.GetSphericalMesh(sfPos0, _sphereCenters[0], _globalScale);

                var sfPos1 = new Vector3i(offset * chunkX, 0, offset * chunkY) + _sphereCenters[1] - new Vector3i(0, (int)_scalarFieldGenerator.AvgElevation, 0);
                Vertex[] data1 = meshGenerator1.GetSphericalMesh(sfPos1, _sphereCenters[1], _globalScale);

                spheres.Add(new Chunk(data0, sfPos0, averageScalarField0, sphere: 0, generateVao));
                spheres.Add(new Chunk(data1, sfPos1, averageScalarField1, sphere: 1, generateVao));
            }
        }

        return spheres;
    }

    private float GetBoundaryValue(Vector3i p)
    {
        float m = 1f;
        return m * p.Y - m * _scalarFieldGenerator.AvgElevation;
    }

    private static float F1(float x, float R, float a = 0.5f, float k = .85f)
        => 0.5f * (MathF.Tanh(a * (x - k * R)) + 1);

    private static float F2(float x, float R, float a = 0.5f, float k = .85f)
        => 0.5f * (-MathF.Tanh(a * (x - k * R)) + 1);
}
