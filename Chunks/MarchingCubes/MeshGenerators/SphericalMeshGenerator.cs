using Chunks.ChunkManagement;
using Chunks.Voxels;
using Common.Meshes;
using OpenTK.Mathematics;

namespace Chunks.MarchingCubes.MeshGenerators;
public class SphericalMeshGenerator : AbstractMeshGenerator
{
    private readonly float _cutoffRadius;

    private readonly Vector3i[] _sphereCenters;

    public SphericalMeshGenerator(float cutoffRadius, Vector3i[] sphereCenters, float isoLevel = 0) : base(isoLevel)
    {
        _cutoffRadius = cutoffRadius;
        _sphereCenters = sphereCenters;
    }

    public override Vertex[] GetMesh(Vector3i chunkPosition, ChunkHandler.ChunkData chunkData)
    {
        var vertices = new List<Vertex>();
        Vector3i sphereCenter = _sphereCenters[chunkData.SphereId];
        Voxel[,,] scalarField = chunkData.Voxels;
        for (int x = 0; x < scalarField.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < scalarField.GetLength(1) - 1; y++)
            {
                for (int z = 0; z < scalarField.GetLength(2) - 1; z++)
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
