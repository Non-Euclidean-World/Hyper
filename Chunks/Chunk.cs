using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities;
using BepuUtilities.Memory;
using Chunks.Voxels;
using Common;
using Common.Meshes;
using OpenTK.Mathematics;
using Mesh = Common.Meshes.Mesh;

namespace Chunks;

public class Chunk
{
    public static int Size = 16;

    public const int Overlap = 2;

    /// <summary>
    /// Size of the voxel field dimension.
    /// </summary>
    public static int TotalSize => Size + Overlap + 1;

    public Vector3i Position { get; }

    public readonly Voxel[,,] Voxels;

    private StaticHandle _handle;

    private TypedIndex _shape;

    public readonly Mesh Mesh;

    public readonly int Sphere;

    public Chunk(Vertex[] vertices, Vector3i position, Voxel[,,] voxels, int sphere = 0, bool createVao = true)
    {
        Voxels = voxels;
        Position = position;
        Mesh = new Mesh(vertices, position, createVao);
        Sphere = sphere;
    }

    public void Render(Shader shader, float scale, float curve, Vector3 cameraPosition) =>
        Mesh.Render(shader, scale, curve, cameraPosition);

    /// <summary>
    /// Mines the selected voxel and all voxels within the radius. Then updates the mesh.
    /// </summary>
    /// <param name="location"></param>
    /// <param name="deltaTime"></param>
    /// <param name="brushWeight"></param>
    /// <param name="radius"></param>
    public void Mine(Vector3 location, float deltaTime, float brushWeight, int radius)
    {
        var x = (int)location.X - Position.X;
        var y = (int)location.Y - Position.Y;
        var z = (int)location.Z - Position.Z;

        for (int xi = Math.Max(0, x - radius); xi <= Math.Min(TotalSize - 1, x + radius); xi++)
        {
            for (int yi = Math.Max(0, y - radius); yi <= Math.Min(TotalSize - 1, y + radius); yi++)
            {
                for (int zi = Math.Max(0, z - radius); zi <= Math.Min(TotalSize - 1, z + radius); zi++)
                {
                    if (DistanceSquared(x, y, z, xi, yi, zi) <= radius * radius)
                    {
                        Voxels[xi, yi, zi].Value += deltaTime * brushWeight * Gaussian(xi, yi, zi, x, y, z, 0.1f);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Fills the selected voxel and all voxels within the radius. Then updates the mesh.
    /// </summary>
    /// <param name="location"></param>
    /// <param name="deltaTime"></param>
    /// <param name="brushWeight"></param>
    /// <param name="radius"></param>
    public void Build(Vector3 location, float deltaTime, float brushWeight, int radius)
    {
        var x = (int)location.X - Position.X;
        var y = (int)location.Y - Position.Y;
        var z = (int)location.Z - Position.Z;

        for (int xi = Math.Max(0, x - radius); xi <= Math.Min(TotalSize - 1, x + radius); xi++)
        {
            for (int yi = Math.Max(0, y - radius); yi <= Math.Min(TotalSize - 1, y + radius); yi++)
            {
                for (int zi = Math.Max(0, z - radius); zi <= Math.Min(TotalSize - 1, z + radius); zi++)
                {
                    if (DistanceSquared(x, y, z, xi, yi, zi) <= radius * radius)
                    {
                        Voxels[xi, yi, zi].Value -= deltaTime * brushWeight * Gaussian(xi, yi, zi, x, y, z, 0.1f);
                    }
                }
            }
        }
    }

    public float DistanceFromChunk(Vector3 location)
    {
        Vector3 chunkTopLeft = Position + new Vector3(TotalSize, TotalSize, TotalSize);

        float closestX = Math.Clamp(location.X, Position.X, chunkTopLeft.X);
        float closestY = Math.Clamp(location.Y, Position.Y, chunkTopLeft.Y);
        float closestZ = Math.Clamp(location.Z, Position.Z, chunkTopLeft.Z);

        Vector3 axisDistances = new Vector3(Math.Abs(closestX - location.X), Math.Abs(closestY - location.Y), Math.Abs(closestZ - location.Z));

        return Math.Max(axisDistances.X, Math.Max(axisDistances.Y, axisDistances.Z));
    }

    public void UpdateCollisionSurface(Simulation simulation, BufferPool bufferPool)
    {
        if (Mesh.Vertices.Length == 0)
        {
            if (_shape.Exists)
            {
                simulation.Shapes.RemoveAndDispose(_shape, bufferPool);
                simulation.Statics.Remove(_handle);
            }
            else return;
        }

        if (!_shape.Exists)
        {
            CreateCollisionSurface(simulation, bufferPool);
            return;
        }

        var collisionSurface = MeshHelper.CreateCollisionSurface(Mesh, bufferPool);
        simulation.Shapes.RemoveAndDispose(_shape, bufferPool);
        _shape = simulation.Shapes.Add(collisionSurface);
        simulation.Statics[_handle].SetShape(_shape);
    }

    public void CreateCollisionSurface(Simulation simulation, BufferPool bufferPool)
    {
        if (Mesh.Vertices.Length == 0)
            return;

        var collisionSurface = MeshHelper.CreateCollisionSurface(Mesh, bufferPool);
        var position = Position;
        _shape = simulation.Shapes.Add(collisionSurface);
        _handle = simulation.Statics.Add(new StaticDescription(
            new System.Numerics.Vector3(position.X, position.Y, position.Z),
            QuaternionEx.Identity,
            _shape));
    }

    public void Dispose(Simulation simulation, BufferPool bufferPool)
    {
        if (_shape.Exists)
        {
            simulation.Shapes.RemoveAndDispose(_shape, bufferPool);
            simulation.Statics.Remove(_handle);
        }
        Mesh.Dispose();
    }

    private static float DistanceSquared(float x1, float y1, float z1, float x2, float y2, float z2)
    {
        return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2);
    }

    private static float Gaussian(float x, float y, float z, float cx, float cy, float cz, float a = 1f)
    {
        return MathF.Exp(-a * ((x - cx) * (x - cx) + (y - cy) * (y - cy) + (z - cz) * (z - cz)));
    }
}
