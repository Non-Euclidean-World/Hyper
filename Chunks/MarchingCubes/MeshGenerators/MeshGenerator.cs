using Chunks.ChunkManagement;
using Chunks.Voxels;
using Common.Meshes;
using OpenTK.Mathematics;

namespace Chunks.MarchingCubes.MeshGenerators;

public class MeshGenerator : BaseMeshGenerator
{
    public MeshGenerator(int seed) : base(seed) { }
    
    public override Vertex[] GetMesh(/*unused*/ Vector3i chunkPosition, ChunkData chunkData)
    {
        Voxel[,,] scalarField = chunkData.Voxels;
        var vertices = new List<Vertex>();
        for (var x = 0; x < scalarField.GetLength(0) - 3; x++)
        {
            for (var y = 0; y < scalarField.GetLength(1) - 3; y++)
            {
                for (var z = 0; z < scalarField.GetLength(2) - 3; z++)
                {
                    vertices.AddRange(GetTriangles(x, y, z, scalarField));
                }
            }
        }

        return vertices.ToArray();
    }
}
