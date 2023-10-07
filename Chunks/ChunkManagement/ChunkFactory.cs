using Chunks.MarchingCubes;
using Chunks.MarchingCubes.MeshGenerators;
using Common.Meshes;
using OpenTK.Mathematics;

namespace Chunks.ChunkManagement;

public class ChunkFactory
{
    private readonly ScalarFieldGenerator _scalarFieldGenerator;

    private readonly MeshGenerator _meshGenerator;

    public float Elevation => _scalarFieldGenerator.AvgElevation;

    public ChunkFactory(ScalarFieldGenerator scalarFieldGenerator, MeshGenerator meshGenerator)
    {
        _scalarFieldGenerator = scalarFieldGenerator;
        _meshGenerator = meshGenerator;
    }

    public Chunk GenerateChunk(Vector3i position, bool generateVao = true)
    {
        var voxels = _scalarFieldGenerator.Generate(Chunk.Size, position);
        Vertex[] data = _meshGenerator.GetMesh(position, new ChunkData { SphereId = 0, Voxels = voxels });

        return new Chunk(data, position, voxels, sphere: 0, generateVao);
    }
}
