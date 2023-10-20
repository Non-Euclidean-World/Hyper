using Chunks.ChunkManagement;
using Chunks.Voxels;
using Common.Meshes;
using OpenTK.Mathematics;

namespace Chunks.MarchingCubes.MeshGenerators;

public class MeshGenerator : BaseMeshGenerator
{
    public override Vertex[] GetMesh(/*unused*/ Vector3i chunkPosition, ChunkData chunkData)
    {
        Voxel[,,] scalarField = chunkData.Voxels;
        var vertices = new List<Vertex>();
        for (var x = 1; x < scalarField.GetLength(0) - 2; x++)
        {
            for (var y = 1; y < scalarField.GetLength(1) - 2; y++)
            {
                for (var z = 1; z < scalarField.GetLength(2) - 2; z++)
                {
                    vertices.AddRange(GetTriangles(x, y, z, scalarField));
                }
            }
        }

        return vertices.ToArray();
    }
}
