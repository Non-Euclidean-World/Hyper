using System.Runtime.InteropServices;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities;
using BepuUtilities.Memory;
using Hyper.Collisions;
using Hyper.MarchingCubes;
using Hyper.MarchingCubes.Voxels;
using NLog;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
namespace Hyper.Meshes;

internal class Chunk : Mesh
{
    public const int Size = 32;

    public new Vector3i Position { get; set; }

    private readonly Voxel[,,] _voxels;

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private StaticHandle _handle;

    private TypedIndex _shape;

    private BepuPhysics.Collidables.Mesh? collisionSurface;
    public Vector3i Center
    {
        get { return new Vector3i(Position.X + Size / 2, Position.Y + Size / 2, Position.Z + Size / 2); }
    }
    public Chunk(Vertex[] vertices, Vector3i position, Voxel[,,] voxels) : base(vertices, position)
    {
        collisionSurface = null;
        _voxels = voxels;
        Position = position;
    }

    /// <summary>
    /// Mines the selected voxel and all voxels within the radius. Then updates the mesh.
    /// </summary>
    /// <param name="location"></param>
    /// <param name="deltaTime"></param>
    /// <param name="radius"></param>
    /// <returns>true is something was mined. false otherwise.</returns>
    public bool Mine(Vector3 location, float deltaTime, float brushWeight, int radius = 5)
    {
        var x = (int)location.X - Position.X;
        var y = (int)location.Y - Position.Y;
        var z = (int)location.Z - Position.Z;
        /*
        if (x < 0 || y < 0 || z < 0
            || x > Size - 1 || y > Size - 1 || z > Size - 1
            || _voxels[x, y, z].Value <= 0f)
            return false;
        */
        if (x < -radius || y < -radius || z < -radius
            || x > Size - 1 + radius || y > Size - 1 + radius || z > Size - 1 + radius)
            return false;
        bool flag = false;
        for (int xi = Math.Max(0, x - radius); xi <= Math.Min(Size - 1, x + radius); xi++)
        {
            for (int yi = Math.Max(0, y - radius); yi <= Math.Min(Size - 1, y + radius); yi++)
            {
                for (int zi = Math.Max(0, z - radius); zi <= Math.Min(Size - 1, z + radius); zi++)
                {
                    if (DistanceSquared(x, y, z, xi, yi, zi) <= radius * radius)
                    {
                        _voxels[xi, yi, zi].Value += deltaTime * brushWeight * Gaussian(xi, yi, zi, x, y, z, 0.1f);
                        flag = true;
                    }
                }
            }
        }

        if(flag) UpdateMesh();

        var error = GL.GetError();
        if (error != ErrorCode.NoError) Logger.Error(error);

        return flag;
    }

    /// <summary>
    /// Fills the selected voxel and all voxels within the radius. Then updates the mesh.
    /// </summary>
    /// <param name="location"></param>
    /// <param name="deltaTime"></param>
    /// <param name="radius"></param>
    /// <returns>true is something was built. false otherwise.</returns>
    public bool Build(Vector3 location, float deltaTime, float brushWeight, int radius = 5)
    {
        var x = (int)location.X - Position.X;
        var y = (int)location.Y - Position.Y;
        var z = (int)location.Z - Position.Z;
        /*
        if (x < 0 || y < 0 || z < 0
            || x > Size - 1 || y > Size - 1 || z > Size - 1
            || _voxels[x, y, z].Value >= 1f)
            return false;
        */
        if (x < -radius || y < -radius || z < -radius
            || x > Size - 1 + radius || y > Size - 1 + radius || z > Size - 1 + radius)
            return false;
        bool flag = false;
        for (int xi = Math.Max(0, x - radius); xi <= Math.Min(Size - 1, x + radius); xi++)
        {
            for (int yi = Math.Max(0, y - radius); yi <= Math.Min(Size - 1, y + radius); yi++)
            {
                for (int zi = Math.Max(0, z - radius); zi <= Math.Min(Size - 1, z + radius); zi++)
                {
                    if (DistanceSquared(x, y, z, xi, yi, zi) <= radius * radius)
                    {
                        _voxels[xi, yi, zi].Value -= deltaTime * brushWeight * Gaussian(xi, yi, zi, x, y, z, 0.1f);
                        flag = true;
                    }
                }
            }
        }

        if(flag) UpdateMesh();

        var error = GL.GetError();
        if (error != ErrorCode.NoError) Logger.Error(error);

        return flag;
    }

    public void UpdateCollisionSurface(Simulation simulation, BufferPool bufferPool)
    {
        if (collisionSurface == null)
        {
            CreateCollisionSurface(simulation, bufferPool);
            return;
        }
        simulation.Shapes.RemoveAndDispose(_shape, bufferPool);
        var mesh = MeshHelper.CreateMeshFromChunk(this, bufferPool);
        _shape = simulation.Shapes.Add(mesh);
        simulation.Statics[_handle].SetShape(_shape);
    }
    public void DisposeCollisionSurface(Simulation simulation, BufferPool bufferPool)
    {        
        //simulation.Statics.Remove(_handle);
        //simulation.Shapes.RemoveAndDispose(_shape, bufferPool); //datarace if in other thread
    }

    public void CreateCollisionSurface(Simulation simulation, BufferPool bufferPool)
    {
        if (vertices.Length <= 3) return;
        var mesh = MeshHelper.CreateMeshFromChunk(this, bufferPool); //throws if chunk contains no actual triangles.
        var position = Position;
        _shape = simulation.Shapes.Add(mesh);
        _handle = simulation.Statics.Add(new StaticDescription(
            new System.Numerics.Vector3(position.X, position.Y, position.Z),
            QuaternionEx.Identity,
            _shape));
        collisionSurface = mesh;
    }

    // Right now this method recreates the whole VAO. This is slow but easier to implement. Will need to be changed to just updating VBO.
    private void UpdateMesh()
    {
        var renderer = new MeshGenerator(_voxels);
        Vertices = renderer.GetMesh();
        NumberOfVertices = Vertices.Length;

        GL.BindVertexArray(VaoId);
        GL.DeleteBuffer(VboId);
        VboId = GL.GenBuffer();

        GL.BindBuffer(BufferTarget.ArrayBuffer, VboId);
        GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * Marshal.SizeOf<Vertex>(), Vertices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf<Vertex>(), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf<Vertex>(), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf<Vertex>(), 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);

        GL.BindVertexArray(0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
    }

    private static float DistanceSquared(float x1, float y1, float z1, float x2, float y2, float z2)
    {
        return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2);
    }

    private static float Gaussian(float x, float y, float z, float cx, float cy, float cz, float a = 1f)
    {
        return MathF.Exp(-a * ((x - cx) * (x - cx) + (y - cy) * (y - cy) + (z - cz) * (z - cz)));
    }

    public static float MinStraightDistance(Vector3 a, Vector3 b)
    {
        return float.Min(float.Min(float.Abs(a.X - b.X), float.Abs(a.Y - b.Y)), float.Abs(a.Z - b.Z));
    }
}