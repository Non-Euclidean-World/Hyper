using Hyper.MarchingCubes;
using NLog;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.Meshes
{
    internal class Chunk : Mesh
    {
        public const int Size = 32;

        public new Vector3i Position;

        public Vector3i Middle => Position + Vector3i.One * Size / 2;

        private float[,,] _voxels;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Chunk(float[] vertices, Vector3i position, float[,,] voxels) : base(vertices, position)
        {
            _voxels = voxels;
            Position = position;
            base.Position = position; // TODO ugly as hell
        }

        private static float DistSqrd(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2);
        }

        private static float Gaussian(float x, float y, float z, float cx, float cy, float cz, float a = 1f)
        {
            return (float)MathHelper.Exp(-a * ((x - cx) * (x - cx) + (y - cy) * (y - cy) + (z - cz) * (z - cz)));
        }

        // This method returns flase if it did not mine anything. True if it did.
        public bool Mine(Vector3 location, float deltaTime, int radius = 5)
        {
            var x = (int)location.X - Position.X;
            var y = (int)location.Y - Position.Y;
            var z = (int)location.Z - Position.Z;

            if (x < 0 || y < 0 || z < 0
                | x > Size - 1 || y > Size - 1 || z > Size - 1
                || _voxels[x, y, z] <= 0f)
                return false;

            float brushWeight = 0.1f;
            for (int xi = x - radius; xi <= x + radius; xi++)
            {
                for (int yi = y - radius; yi <= y + radius; yi++)
                {
                    for (int zi = z - radius; zi <= z + radius; zi++)
                    {
                        if (DistSqrd(x, y, z, xi, yi, zi) <= radius * radius)
                        {
                            _voxels[xi, yi, zi] += deltaTime * brushWeight * Gaussian(xi, yi, zi, x, y, z, 0.1f);
                        }
                    }
                }
            }

            _logger.Info($"Mined block at {x},{y},{z}");

            UpdateMesh();

            var error = GL.GetError();
            if (error != ErrorCode.NoError) _logger.Error(error);
            return true;
        }

        public bool Build(Vector3 location, float deltaTime, int radius = 5)
        {
            var x = (int)location.X - Position.X;
            var y = (int)location.Y - Position.Y;
            var z = (int)location.Z - Position.Z;

            if (x < 0 || y < 0 || z < 0
                || x > Size - 1 || y > Size - 1 || z > Size - 1
                || _voxels[x, y, z] >= 1f)
                return false;

            float brushWeight = 0.1f;
            for (int xi = x - radius; xi <= x + radius; xi++)
            {
                for (int yi = y - radius; yi <= y + radius; yi++)
                {
                    for (int zi = z - radius; zi <= z + radius; zi++)
                    {
                        if (DistSqrd(x, y, z, xi, yi, zi) <= radius * radius)
                        {
                            _voxels[xi, yi, zi] -= deltaTime * brushWeight * Gaussian(xi, yi, zi, x, y, z, 0.1f);
                        }
                    }
                }
            }
            _logger.Info($"Built block at {x},{y},{z}");

            UpdateMesh();

            var error = GL.GetError();
            if (error != ErrorCode.NoError) _logger.Error(error);
            return true;
        }

        // Right now this method recreates the whole VAO. This is slow but easier to implement. Will need to be changed to just updating VBO.
        private void UpdateMesh()
        {
            var renderer = new Renderer(_voxels, Position);
            Triangle[] triangles = renderer.GetMesh();
            float[] vertices = Generator.GetTriangleAndNormalData(triangles);

            GL.BindVertexArray(VaoId);
            GL.DeleteBuffer(VboId);
            VboId = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VboId);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
    }
}
