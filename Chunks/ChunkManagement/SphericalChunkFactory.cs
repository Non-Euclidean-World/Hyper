using Chunks.MarchingCubes;
using Chunks.MarchingCubes.MeshGenerators;
using Chunks.Voxels;
using Common.Meshes;
using OpenTK.Mathematics;

namespace Chunks.ChunkManagement;
public class SphericalChunkFactory
{
    private readonly ScalarFieldGenerator _scalarFieldGenerator;

    private readonly Vector3i[] _sphereCenters;

    private readonly float _globalScale;

    private readonly SphericalMeshGenerator _meshGenerator;

    public SphericalChunkFactory(ScalarFieldGenerator scalarFieldGenerator, Vector3i[] sphereCenters, float globalScale, SphericalMeshGenerator meshGenerator)
    {
        _scalarFieldGenerator = scalarFieldGenerator;
        _sphereCenters = sphereCenters;
        _globalScale = globalScale;
        _meshGenerator = meshGenerator;
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
                var averageScalarField0 = new Voxel[Chunk.Size + 1, Chunk.Size + 1, Chunk.Size + 1];
                var averageScalarField1 = new Voxel[Chunk.Size + 1, Chunk.Size + 1, Chunk.Size + 1];

                int offset = Chunk.Size;

                var sf0Pos = new Vector3i(offset * chunkX, 0, offset * chunkY) + _sphereCenters[0];
                var sf1Pos = new Vector3i(offset * chunkX, 0, offset * chunkY) + _sphereCenters[1];

                var scalarField0 = _scalarFieldGenerator.Generate(Chunk.Size, sf0Pos);
                var scalarField1 = _scalarFieldGenerator.Generate(Chunk.Size, sf1Pos);

                for (int x = 0; x < Chunk.Size + 1; x++)
                {
                    for (int y = 0; y < Chunk.Size + 1; y++)
                    {
                        for (int z = 0; z < Chunk.Size + 1; z++)
                        {
                            var position = new Vector3i(x + chunkX * Chunk.Size, y, z + chunkY * Chunk.Size);
                            var radius = MathF.PI / 2 / _globalScale;
                            var boundaryValue = GetBoundaryValue(position);

                            var d = Vector3.Distance(position, _sphereCenters[0]);
                            averageScalarField0[x, y, z].Value = SmoothStepUp(d, radius) * boundaryValue + SmoothStepDown(d, radius) * scalarField0[x, y, z].Value;
                            averageScalarField1[x, y, z].Value = SmoothStepUp(d, radius) * boundaryValue + SmoothStepDown(d, radius) * scalarField1[x, y, z].Value;

                            VoxelType type;
                            if (y < Chunk.Size / 2.5) type = VoxelType.Rock;
                            else if (y < Chunk.Size / 2) type = VoxelType.GrassRock;
                            else type = VoxelType.Grass;

                            averageScalarField0[x, y, z].Type = averageScalarField1[x, y, z].Type = type;
                        }
                    }
                }

                var sfPos0 = new Vector3i(offset * chunkX, 0, offset * chunkY) + _sphereCenters[0] - new Vector3i(0, (int)_scalarFieldGenerator.AvgElevation, 0);
                Vertex[] data0 = _meshGenerator.GetMesh(sfPos0, new ChunkData { SphereId = 0, Voxels = averageScalarField0 });

                var sfPos1 = new Vector3i(offset * chunkX, 0, offset * chunkY) + _sphereCenters[1] - new Vector3i(0, (int)_scalarFieldGenerator.AvgElevation, 0);
                Vertex[] data1 = _meshGenerator.GetMesh(sfPos1, new ChunkData { SphereId = 1, Voxels = averageScalarField1 });

                spheres.Add(new Chunk(data0, sfPos0, averageScalarField0, sphere: 0, generateVao));
                spheres.Add(new Chunk(data1, sfPos1, averageScalarField1, sphere: 1, generateVao));
            }
        }

        return spheres;
    }

    private float GetBoundaryValue(Vector3i p)
    {
        var m = 1f;
        return m * p.Y - m * _scalarFieldGenerator.AvgElevation;
    }

    private static float SmoothStepUp(float x, float r, float a = 0.5f, float k = .85f)
        => 0.5f * (MathF.Tanh(a * (x - k * r)) + 1);

    private static float SmoothStepDown(float x, float r, float a = 0.5f, float k = .85f)
        => 0.5f * (-MathF.Tanh(a * (x - k * r)) + 1);
}
