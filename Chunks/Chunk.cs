using System.Runtime.InteropServices;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities;
using BepuUtilities.Memory;
using Chunks.MarchingCubes;
using Chunks.Voxels;
using Common.Meshes;
using NLog;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Mesh = Common.Meshes.Mesh;

namespace Chunks;

public class Chunk : Mesh
{
    public const int Size = 32;

    public new Vector3i Position { get; set; }

    private readonly Voxel[,,] _voxels;

    // TODO No clue if logging works after moving it to Common project. Need to check.
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private StaticHandle _handle;

    private TypedIndex _shape;

    public Chunk(Vertex[] vertices, Vector3i position, Voxel[,,] voxels) : base(vertices, position)
    {
        _voxels = voxels;
        Position = position;
    }

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
                        _voxels[xi, yi, zi].Value += deltaTime * brushWeight * Gaussian(xi, yi, zi, x, y, z, 0.1f);
                    }
                }
            }
        }

        UpdateMesh();

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
                        _voxels[xi, yi, zi].Value -= deltaTime * brushWeight * Gaussian(xi, yi, zi, x, y, z, 0.1f);
                    }
                }
            }
        }

        UpdateMesh();

        var error = GL.GetError();
        if (error != ErrorCode.NoError) Logger.Error(error);

        return true;
    }

    public void UpdateCollisionSurface(Simulation simulation, BufferPool bufferPool)
    {
        simulation.Shapes.RemoveAndDispose(_shape, bufferPool);
        var mesh = MeshHelper.CreateMeshFromChunk(this, bufferPool);
        _shape = simulation.Shapes.Add(mesh);
        simulation.Statics[_handle].SetShape(_shape);
    }

    public void CreateCollisionSurface(Simulation simulation, BufferPool bufferPool)
    {
        var mesh = MeshHelper.CreateMeshFromChunk(this, bufferPool);
        var position = Position;
        _shape = simulation.Shapes.Add(mesh);
        _handle = simulation.Statics.Add(new StaticDescription(
            new System.Numerics.Vector3(position.X, position.Y, position.Z),
            QuaternionEx.Identity,
            _shape));
    }

    private void UpdateMesh()
    {
        var renderer = new MeshGenerator(_voxels);
        Vertices = renderer.GetMesh();
        NumberOfVertices = Vertices.Length;

        GL.BindVertexArray(VaoId);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VboId);
        GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * Marshal.SizeOf<Vertex>(), IntPtr.Zero, BufferUsageHint.StaticDraw);
        IntPtr ptr = GL.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.WriteOnly);
        unsafe
        {
            fixed (Vertex* source = Vertices)
            {
                System.Buffer.MemoryCopy(source, ptr.ToPointer(), Vertices.Length * Marshal.SizeOf<Vertex>(), Vertices.Length * Marshal.SizeOf<Vertex>());
            }
        }
        GL.UnmapBuffer(BufferTarget.ArrayBuffer);
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
