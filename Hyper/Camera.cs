using Hyper.Command;
using NLog;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper
{
    public class Camera : Commandable
    {
        private Vector3 _front = -Vector3.UnitZ;

        private Vector3 _up = Vector3.UnitY;

        private Vector3 _right = Vector3.UnitX;

        private const float _curve = 1f;

        private float _pitch;

        private float _yaw = -MathHelper.PiOver2;

        private float _fov = MathHelper.PiOver2;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Camera(Vector3 position, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
        }

        public Vector3 Position { get; set; }

        public float AspectRatio { private get; set; }

        public Vector3 Front => _front;

        public Vector3 Up => _up;

        public Vector3 Right => _right;

        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(_pitch);
            set
            {
                var angle = MathHelper.Clamp(value, -89f, 89f);
                _pitch = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }

        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(_yaw);
            set
            {
                _yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }

        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 90f);
                _fov = MathHelper.DegreesToRadians(angle);
            }
        }

        // The camera is defined by eye position e
        // and three orthogonal unit vectors in the tangent space of the eye, right direction i
        // , up direction j, and negative view direction k. L is the curvature
        //
        // | i_x     j_x     k_x     Le_x |
        // | i_y     j_y     k_y     Le_y |
        // | i_z     j_z     k_z     Le_z |
        // | Li_w    Lj_w    Lk_w    e_w  |
        public Matrix4 GetViewMatrix()
        {
            //return new Matrix4(
            //    _right.X, _up.X, _front.X, _curve * Position.X,
            //    _right.Y, _up.Y, _front.Y, _curve * Position.Y,
            //    _right.Z, _up.Z, _front.Z, _curve * Position.Z,
            //    _curve * _right.w, _curve * _up.w, _curve * _front.W, Position.W
            //);

            return Matrix4.LookAt(Position, Position + _front, _up);
        }

        // | 1/s_x  0      0    0 |
        // | 0      1/s_y  0    0 |
        // | 0      0      a   -1 |
        // | 0      0      0    b |
        //
        // Euclidean geometry
        // aE = -dmin + dmax / (dmax - dmin);
        // bE = -2 * dmin * dmax / (dmax - dmin);

        // Hyperbolic geometry
        // aH = -Sinh(dmin + dmax) / Sinh(dmax - dmin);
        // bH = -2 * Sinh(dmin) * Sinh(dmax) / Sinh(dmax - dmin);

        // Elliptic geometry
        // aS = -Sin(dmin + dmax) / Sin(dmax - dmin);
        // bS = -2 * Sin(dmin) * Sin(dmax) / Sin(dmax - dmin);
        public Matrix4 GetProjectionMatrix()
        {
            //float dmin = 0.01f;
            //float dmax = 100f;

            //// Euclidean geometry
            //float a = -dmin + dmax / (dmax - dmin);
            //float b = -2 * dmin * dmax / (dmax - dmin);

            //// Hyperbolic geometry
            //float a = -Math.Sinh(dmin + dmax) / Math.Sinh(dmax - dmin);
            //float b = -2 * Math.Sinh(dmin) * Math.Sinh(dmax) / Math.Sinh(dmax - dmin);

            //// Elliptic geometry
            //float a = -Math.Sin(dmin + dmax) / Math.Sin(dmax - dmin);
            //float b = -2 * Math.Sin(dmin) * Math.Sin(dmax) / Math.Sin(dmax - dmin);

            //return new Matrix4(
            //    1f / AspectRatio, 0f, 0f, 0f,
            //    0f, 1f / AspectRatio, 0f, 0f,
            //    0f, 0f, a, -1f,
            //    0f, 0f, 0f, b
            //);

            return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
        }

        // Translation by vector q
        // | 1 - (L* q_x * q_x) / (1 + q_w),   - (L* q_x * q_y) / (1 + q_w),      - (L* q_x * q_z) / (1 + q_w),      -L * q_x |
        // | - (L* q_y * q_x) / (1 + q_w),     1 - (L* q_y * q_y) / (1 + q_w),    - (L* q_y * q_z) / (1 + q_w),      -L * q_y |
        // | - (L* q_z * q_x) / (1 + q_w),     - (L* q_z * q_y) / (1 + q_w),      1 - (L* q_z * q_z) / (1 + q_w),    -L * q_z |
        // | q_x,                              q_y,                               q_z,                                    q_w |

        public void UpdatePosition(Vector3 move)
        {
            // float move_w = MathF.Sqrt(move.X * move.X + move.Y * move.Y + move.Z * move.Z + 1);

            // var matrix = new Matrix4(
            //     1 - (_curve * move.X * move.X) / (1 + move_w), - (_curve * move.X * move.Y) / (1 + move_w), - (_curve * move.X * move.Z) / (1 + move_w), -_curve * move.X,
            //     - (_curve * move.Y * move.X) / (1 + move_w), 1 - (_curve * move.Y * move.Y) / (1 + move_w), - (_curve * move.Y * move.Z) / (1 + move_w), -_curve * move.Y,
            //     - (_curve * move.Z * move.X) / (1 + move_w), - (_curve * move.Z * move.Y) / (1 + move_w), 1 - (_curve * move.Z * move.Z) / (1 + move_w), -_curve * move.Z,
            //     move.X, move.Y, move.Z, move_w
            // );
            // Position = Position * matrix;
            
            Position += move;
        }

        private void UpdateVectors()
        {
            _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
            _front.Y = MathF.Sin(_pitch);
            _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

            _front = Vector3.Normalize(_front);

            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }

        public void Move(KeyboardState input, float time)
        {
            const float cameraSpeed = 1.5f;

            Vector3 move = Vector3.Zero;

            if (input.IsKeyDown(Keys.W))
            {
                move += Front * cameraSpeed * time; // Forward
            }
            if (input.IsKeyDown(Keys.S))
            {
                move -= Front * cameraSpeed * time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                move -= Right * cameraSpeed * time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                move += Right * cameraSpeed * time; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                move += Up * cameraSpeed * time; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                move -= Up * cameraSpeed * time; // Down
            }

            UpdatePosition(move);
        }

        protected override void SetComamnd(string[] args)
        {
            switch (args[0])
            {
                case "fov":
                    Fov = int.Parse(args[1]);
                    break;
            }
        }

        protected override void GetComamnd(string[] args)
        {
            switch (args[0])
            {
                case "fov":
                    Console.WriteLine(Fov);
                    break;
                case "position":
                    Console.WriteLine(Position);
                    break;
            }
        }
    }
}