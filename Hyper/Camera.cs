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

        private float _curve = 0f;

        public float Curve { get => _curve; set => _curve = value; }

        private float _pitch;

        private float _yaw = -MathHelper.PiOver2;

        private float _fov = MathHelper.PiOver2;

        private float _cameraSpeed = 50f;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Camera(Vector3 position, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
        }

        public Vector3 Position { get; /*set;*/ }

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

        public Matrix4 GetViewMatrix()
        {
            Matrix4 V = Matrix4.LookAt(Position, Position + _front, _up);
            Vector4 ic = new Vector4(V.Column0.Xyz, 0);
            Vector4 jc = new Vector4(V.Column1.Xyz, 0);
            Vector4 kc = new Vector4(V.Column2.Xyz, 0);

            Vector4 geomEye = PortEucToCurved(Position);

            Matrix4 eyeTranslate = TranslateMatrix(geomEye);
            Vector4 icp = ic * eyeTranslate;
            Vector4 jcp = jc * eyeTranslate;
            Vector4 kcp = kc * eyeTranslate;

            if (MathHelper.Abs(_curve) < 0.001)
            {
                return V;
            }

            Matrix4 nonEuclidView = new Matrix4(
                icp.X, jcp.X, kcp.X, _curve * geomEye.X,
                icp.Y, jcp.Y, kcp.Y, _curve * geomEye.Y,
                icp.Z, jcp.Z, kcp.Z, _curve * geomEye.Z,
                _curve * icp.W, _curve * jcp.W, _curve * kcp.W, geomEye.W);
            //nonEuclidView.Transpose();
            return nonEuclidView;

        }

        public Matrix4 GetProjectionMatrix()
        {
            Matrix4 P = Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
            float sFovX = P.Column0.X;
            float sFovY = P.Column1.Y;
            float fp = 0.01f; // scale front clipping plane according to the global scale factor of the scene

            if (_curve <= 0.00001)
            {
                return P;
            }
            Matrix4 nonEuclidProj = new Matrix4(
                sFovX, 0, 0, 0,
                0, sFovY, 0, 0,
                0, 0, 0, -1,
                0, 0, -fp, 0
                );
            //nonEuclidProj.Transpose();
            return nonEuclidProj;
        }

        public Vector4 PortEucToCurved(Vector3 eucPoint)
        {
            return PortEucToCurved(new Vector4(eucPoint, 1));
        }

        private Vector4 PortEucToCurved(Vector4 eucPoint)
        {
            Vector3 p = eucPoint.Xyz;
            float dist = p.Length;
            if (dist < 0.0001f) return eucPoint;
            if (_curve > 0) return new Vector4(p / dist * (float)MathHelper.Sin(dist), (float)MathHelper.Cos(dist));
            if (_curve < 0) return new Vector4(p / dist * (float)MathHelper.Sinh(dist), (float)MathHelper.Cosh(dist));
            return eucPoint;
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

        private Matrix4 TranslateMatrix(Vector4 to)
        {
            return TranslateMatrix(to, new Vector4(0, 0, 0, 1));
        }

        private Matrix4 TranslateMatrix(Vector4 to, Vector4 g)
        {
            Matrix4 T;
            if (_curve != 0)
            {
                float denom = 1 + to.W;
                T = new Matrix4(
                    1 - _curve * to.X * to.X / denom, -_curve * to.X * to.Y / denom, -_curve * to.X * to.Z / denom, -_curve * to.X,
                    -_curve * to.Y * to.X / denom, 1 - _curve * to.Y * to.Y / denom, -_curve * to.Y * to.Z / denom, -_curve * to.Y,
                    -_curve * to.Z * to.X / denom, -_curve * to.Z * to.Y / denom, 1 - _curve * to.Z * to.Z / denom, -_curve * to.Z,
                    to.X, to.Y, to.Z, to.W);
            }
            else
            {
                T = new Matrix4(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                to.X, to.Y, to.Z, 1);
            }

            //T.Transpose();
            return T;
        }

        public Vector3 Move(KeyboardState input, float time)
        {
            float cameraSpeed = _cameraSpeed;

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

            /*UpdatePosition(move);*/
            return move;
        }

        /*private void UpdatePosition(Vector3 move)
        {
            Position += move;
        }*/

        protected override void SetComamnd(string[] args)
        {
            switch (args[0])
            {
                case "fov":
                    Fov = int.Parse(args[1]);
                    break;
                case "curve":
                    if (args[1] == "h")
                        _curve = -1f;
                    if (args[1] == "s")
                        _curve = 1f;
                    if (args[1] == "e")
                        _curve = 0f;
                    break;
                case "speed":
                    _cameraSpeed = float.Parse(args[1]);
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