﻿using Hyper.Meshes;
using OpenTK.Mathematics;

namespace Hyper.MarchingCubes
{
    internal class ChunkFactory
    {
        private readonly ScalarFieldGenerator _scalarFieldGenerator;

        public ChunkFactory(ScalarFieldGenerator scalarFieldGenerator)
        {
            _scalarFieldGenerator = scalarFieldGenerator;
        }

        public Chunk GenerateChunk(Vector3i position)
        {
            var scalarField = _scalarFieldGenerator.Generate(Chunk.Size, position);
            var renderer = new MeshGenerator(scalarField);
            Vertex[] data = renderer.GetMesh();

            return new Chunk(data, position, scalarField);
        }
    }
}
