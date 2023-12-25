﻿using Chunks.ChunkManagement;
using Chunks.Voxels;
using Common.Meshes;
using OpenTK.Mathematics;

namespace Chunks.MarchingCubes.MeshGenerators;
public class SphericalMeshGenerator : BaseMeshGenerator
{
    private readonly float _cutoffRadius;

    private readonly Vector3i[] _sphereCenters;

    public SphericalMeshGenerator(float cutoffRadius, Vector3i[] sphereCenters, int seed) : base(seed)
    {
        _cutoffRadius = cutoffRadius;
        _sphereCenters = sphereCenters;
    }

    public override Vertex[] GetMesh(Vector3i chunkPosition, ChunkData chunkData)
    {
        var vertices = new List<Vertex>();
        Vector3i sphereCenter = _sphereCenters[chunkData.SphereId];
        Voxel[,,] scalarField = chunkData.Voxels;
        for (int x = 0; x < scalarField.GetLength(0) - 3; x++)
        {
            for (int y = 0; y < scalarField.GetLength(1) - 3; y++)
            {
                for (int z = 0; z < scalarField.GetLength(2) - 3; z++)
                {
                    var xAbs = chunkPosition.X + x - sphereCenter.X;
                    var yAbs = chunkPosition.Y + y - sphereCenter.Y;
                    var zAbs = chunkPosition.Z + z - sphereCenter.Z;
                    if (xAbs * xAbs + yAbs * yAbs + zAbs * zAbs >= _cutoffRadius * _cutoffRadius)
                        continue;

                    vertices.AddRange(GetTriangles(x, y, z, chunkData.Voxels));
                }
            }
        }

        return vertices.ToArray();
    }
}
