using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using Physics.TypingUtils;
using BepuMesh = BepuPhysics.Collidables.Mesh;
using Mesh = Common.Meshes.Mesh;

namespace Chunks;
internal static class MeshHelper
{
    /// <summary>
    /// Creates a mesh shape from a mesh.
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="pool"></param>
    /// <returns></returns>
    public static BepuMesh CreateCollisionSurface(Mesh mesh, BufferPool pool)
    {
        int triangleCount = mesh.Vertices.Length / 3;
        pool.Take<Triangle>(triangleCount, out var triangles);

        for (int i = 0; i < triangleCount; i++)
        {
            ref var triangle = ref triangles[i];

            triangle.A = Conversions.ToNumericsVector(mesh.Vertices[3 * i].Position);
            triangle.B = Conversions.ToNumericsVector(mesh.Vertices[3 * i + 1].Position);
            triangle.C = Conversions.ToNumericsVector(mesh.Vertices[3 * i + 2].Position);
        }

        return new BepuMesh(triangles, new System.Numerics.Vector3(1f, 1f, 1f), pool);
    }
}
