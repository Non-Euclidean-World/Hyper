using System.Runtime.InteropServices;
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
    /// <param name="radius"></param>
    /// <returns>true is something was mined. false otherwise.</returns>
    public bool Mine(Vector3 location, float deltaTime, int radius = 5)
    {
        var x = (int)location.X - Position.X;
        var y = (int)location.Y - Position.Y;
        var z = (int)location.Z - Position.Z;

        if (x < 0 || y < 0 || z < 0
            || x > Size - 1 || y > Size - 1 || z > Size - 1
            || _voxels[x, y, z].Value <= 0f)
            return false;

        float brushWeight = 1f;
        for (int xi = Math.Max(0, x - radius); xi <= Math.Min(Size - 1, x + radius); xi++)
        {
            for (int yi = Math.Max(0, y - radius); yi <= Math.Min(Size - 1, y + radius); yi++)
            {
                for (int zi = Math.Max(0, z - radius); zi <= Math.Min(Size - 1, z + radius); zi++)
                {
                    if (DistSqrd(x, y, z, xi, yi, zi) <= radius * radius)
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
    /// <param name="radius"></param>
    /// <returns>true is something was built. false otherwise.</returns>
    public bool Build(Vector3 location, float deltaTime, int radius = 5)
    {
        var x = (int)location.X - Position.X;
        var y = (int)location.Y - Position.Y;
        var z = (int)location.Z - Position.Z;

        if (x < 0 || y < 0 || z < 0
            || x > Size - 1 || y > Size - 1 || z > Size - 1
            || _voxels[x, y, z].Value >= 1f)
            return false;

        float brushWeight = 1f;
        for (int xi = Math.Max(0, x - radius); xi <= Math.Min(Size - 1, x + radius); xi++)
        {
            for (int yi = Math.Max(0, y - radius); yi <= Math.Min(Size - 1, y + radius); yi++)
            {
                for (int zi = Math.Max(0, z - radius); zi <= Math.Min(Size - 1, z + radius); zi++)
                {
                    if (DistSqrd(x, y, z, xi, yi, zi) <= radius * radius)
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

    // Right now this method recreates the whole VAO. This is slow but easier to implement. Will need to be changed to just updating VBO.
    private void UpdateMesh()
    {
        var renderer = new MeshGenerator(_voxels);
        Vertex[] vertices = renderer.GetMesh();
        NumberOfVertices = vertices.Length;

        GL.BindVertexArray(VaoId);
        GL.DeleteBuffer(VboId);
        VboId = GL.GenBuffer();

        GL.BindBuffer(BufferTarget.ArrayBuffer, VboId);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Marshal.SizeOf<Vertex>(), vertices, BufferUsageHint.StaticDraw);

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

    private static float DistSqrd(float x1, float y1, float z1, float x2, float y2, float z2)
    {
        return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2);
    }

    private static float Gaussian(float x, float y, float z, float cx, float cy, float cz, float a = 1f)
    {
        return (float)MathHelper.Exp(-a * ((x - cx) * (x - cx) + (y - cy) * (y - cy) + (z - cz) * (z - cz)));
    }
}
