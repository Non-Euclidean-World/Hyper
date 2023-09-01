using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using Physics.TypingUtils;

namespace Chunks;
internal static class MeshHelper
{
    /// <summary>
    /// Creates a mesh shape from a chunk 
    /// </summary>
    /// <param name="chunk"></param>
    /// <param name="pool"></param>
    /// <returns></returns>
    public static BepuPhysics.Collidables.Mesh CreateMeshFromChunk(Chunk chunk, BufferPool pool)
    {
        int triangleCount = chunk.Vertices.Length / 3;
        pool.Take<Triangle>(triangleCount, out var triangles);

        for (int i = 0; i < triangleCount; i++)
        {
            ref var triangle = ref triangles[i];

            triangle.A = Conversions.ToNumericsVector(chunk.Vertices[3 * i].Position);
            triangle.B = Conversions.ToNumericsVector(chunk.Vertices[3 * i + 1].Position);
            triangle.C = Conversions.ToNumericsVector(chunk.Vertices[3 * i + 2].Position);
        }

        return new BepuPhysics.Collidables.Mesh(triangles, new System.Numerics.Vector3(1f, 1f, 1f), pool);
    }
}
