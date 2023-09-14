using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities;
using BepuUtilities.Memory;
using Chunks.MarchingCubes;
using Chunks.Voxels;
using Common;
using Common.Meshes;
using NLog;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Mesh = Common.Meshes.Mesh;

namespace Chunks;

public class Chunk
{
    public const int Size = 32;

    public Vector3i Position { get; }

    public readonly Voxel[,,] Voxels;

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private StaticHandle _handle;

    private TypedIndex _shape;

    public Mesh Mesh;

    public Chunk(Vertex[] vertices, Vector3i position, Voxel[,,] voxels, bool createVao = true)
    {
        Voxels = voxels;
        Position = position;
        Mesh = new Mesh(vertices, position, createVao);
    }

    public void Render(Shader shader, float scale, Vector3 cameraPosition) =>
        Mesh.Render(shader, scale, cameraPosition);

    /// <summary>
    /// Mines the selected voxel and all voxels within the radius. Then updates the mesh.
    /// </summary>
    /// <param name="location"></param>
    /// <param name="deltaTime"></param>
    /// <param name="brushWeight"></param>
    /// <param name="radius"></param>
    /// <returns>true is something was mined. false otherwise.</returns>
    public bool Mine(Vector3 location, float deltaTime, float brushWeight, int radius = 5)
    {
        var x = (int)location.X - Position.X;
        var y = (int)location.Y - Position.Y;
        var z = (int)location.Z - Position.Z;

        if (x < 0 || y < 0 || z < 0
            || x > Size - 1 || y > Size - 1 || z > Size - 1)
            return false;

        for (int xi = Math.Max(0, x - radius); xi <= Math.Min(Size - 1, x + radius); xi++)
        {
            for (int yi = Math.Max(0, y - radius); yi <= Math.Min(Size - 1, y + radius); yi++)
            {
                for (int zi = Math.Max(0, z - radius); zi <= Math.Min(Size - 1, z + radius); zi++)
                {
                    if (DistanceSquared(x, y, z, xi, yi, zi) <= radius * radius)
                    {
                        Voxels[xi, yi, zi].Value += deltaTime * brushWeight * Gaussian(xi, yi, zi, x, y, z, 0.1f);
                    }
                }
            }
        }

        var renderer = new MeshGenerator(Voxels);
        Mesh.Update(renderer.GetMesh());

        var error = GL.GetError();
        if (error != ErrorCode.NoError) Logger.Error(error);

        return true;
    }

    /// <summary>
    /// Fills the selected voxel and all voxels within the radius. Then updates the mesh.
    /// </summary>
    /// <param name="location"></param>
    /// <param name="deltaTime"></param>
    /// <param name="brushWeight"></param>
    /// <param name="radius"></param>
    /// <returns>true is something was built. false otherwise.</returns>
    public bool Build(Vector3 location, float deltaTime, float brushWeight, int radius = 5)
    {
        var x = (int)location.X - Position.X;
        var y = (int)location.Y - Position.Y;
        var z = (int)location.Z - Position.Z;

        if (x < 0 || y < 0 || z < 0
            || x > Size - 1 || y > Size - 1 || z > Size - 1)
            return false;

        for (int xi = Math.Max(0, x - radius); xi <= Math.Min(Size - 1, x + radius); xi++)
        {
            for (int yi = Math.Max(0, y - radius); yi <= Math.Min(Size - 1, y + radius); yi++)
            {
                for (int zi = Math.Max(0, z - radius); zi <= Math.Min(Size - 1, z + radius); zi++)
                {
                    if (DistanceSquared(x, y, z, xi, yi, zi) <= radius * radius)
                    {
                        Voxels[xi, yi, zi].Value -= deltaTime * brushWeight * Gaussian(xi, yi, zi, x, y, z, 0.1f);
                    }
                }
            }
        }

        var renderer = new MeshGenerator(Voxels);
        Mesh.Update(renderer.GetMesh());

        var error = GL.GetError();
        if (error != ErrorCode.NoError) Logger.Error(error);

        return true;
    }

    public void UpdateCollisionSurface(Simulation simulation, BufferPool bufferPool)
    {
        var collisionSurface = MeshHelper.CreateCollisionSurface(Mesh, bufferPool);
        simulation.Shapes.RemoveAndDispose(_shape, bufferPool);
        _shape = simulation.Shapes.Add(collisionSurface);
        simulation.Statics[_handle].SetShape(_shape);
    }

    public void CreateCollisionSurface(Simulation simulation, BufferPool bufferPool)
    {
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
