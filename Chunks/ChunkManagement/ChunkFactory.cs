﻿using Chunks.MarchingCubes;
using Common.Meshes;
using OpenTK.Mathematics;

namespace Chunks.ChunkManagement;

public class ChunkFactory
{
    private readonly ScalarFieldGenerator _scalarFieldGenerator;

    public float Elevation => _scalarFieldGenerator.AvgElevation;

    public ChunkFactory(ScalarFieldGenerator scalarFieldGenerator)
    {
        _scalarFieldGenerator = scalarFieldGenerator;
    }

    public Chunk GenerateChunk(Vector3i position, bool generateVao = true)
    {
        var voxels = _scalarFieldGenerator.Generate(Chunk.Size, position);
        var meshGenerator = new MeshGenerator(voxels);
        Vertex[] data = meshGenerator.GetMesh();

        return new Chunk(data, position, voxels, generateVao);
    }
}
