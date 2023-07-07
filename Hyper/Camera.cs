using Hyper.Command;
using NLog;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper
{
    public class Camera : Commandable
    {
        public float Curve { get; set; } = 0f;

        public Vector3 ReferencePointPosition { get; set; } = Vector3.Zero;

        private Vector3 _front = -Vector3.UnitZ;

        private Vector3 _up = Vector3.UnitY;

        private Vector3 _right = Vector3.UnitX;

        private float _pitch;

        private float _yaw = -MathHelper.PiOver2;

        private float _fov = MathHelper.PiOver2;

        private float _cameraSpeed = 50f;

        private float _near;

        private float _far;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Camera(float aspectRatio, float near, float far)
        {
            AspectRatio = aspectRatio;
            _near = near;
            _far = far;
        }

        private Vector3 _position = Vector3.UnitY;

        public float AspectRatio { private get; set; }

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
            Matrix4 V = Matrix4.LookAt(_position, _position + _front, _up);
            Vector4 ic = new Vector4(V.Column0.Xyz, 0);
            Vector4 jc = new Vector4(V.Column1.Xyz, 0);
            Vector4 kc = new Vector4(V.Column2.Xyz, 0);

            Vector4 geomEye = PortEucToCurved(_position);

            Matrix4 eyeTranslate = TranslateMatrix(geomEye);
            Vector4 icp = ic * eyeTranslate;
            Vector4 jcp = jc * eyeTranslate;
            Vector4 kcp = kc * eyeTranslate;

            if (MathHelper.Abs(Curve) < Constants.Eps)
            {
                return V;
            }

            Matrix4 nonEuclidView = new Matrix4(
                icp.X, jcp.X, kcp.X, Curve * geomEye.X,
                icp.Y, jcp.Y, kcp.Y, Curve * geomEye.Y,
                icp.Z, jcp.Z, kcp.Z, Curve * geomEye.Z,
                Curve * icp.W, Curve * jcp.W, Curve * kcp.W, geomEye.W);

            return nonEuclidView;
        }

        public Matrix4 GetProjectionMatrix()
        {
            Matrix4 P = Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, _near, _far);
            float sFovX = P.Column0.X;
            float sFovY = P.Column1.Y;
            float fp = _near; // scale front clipping plane according to the global scale factor of the scene

            if (Curve <= Constants.Eps)
            {
                return P;
            }
            Matrix4 nonEuclidProj = new Matrix4(
                sFovX, 0, 0, 0,
                0, sFovY, 0, 0,
                0, 0, 0, -1,
                0, 0, -fp, 0
                );

            return nonEuclidProj;
        }

        public Matrix4 TranslateMatrix(Vector4 to)
        {
            Matrix4 T;
            if (MathHelper.Abs(Curve) < Constants.Eps)
            {
                T = new Matrix4(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                to.X, to.Y, to.Z, 1);
            }
            else
            {
                float denom = 1 + to.W;
                T = new Matrix4(
                    1 - Curve * to.X * to.X / denom, -Curve * to.X * to.Y / denom, -Curve * to.X * to.Z / denom, -Curve * to.X,
                    -Curve * to.Y * to.X / denom, 1 - Curve * to.Y * to.Y / denom, -Curve * to.Y * to.Z / denom, -Curve * to.Y,
                    -Curve * to.Z * to.X / denom, -Curve * to.Z * to.Y / denom, 1 - Curve * to.Z * to.Z / denom, -Curve * to.Z,
                    to.X, to.Y, to.Z, to.W);
            }

            return T;
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
            if (Curve > 0) return new Vector4(p / dist * (float)MathHelper.Sin(dist), (float)MathHelper.Cos(dist));
            if (Curve < 0) return new Vector4(p / dist * (float)MathHelper.Sinh(dist), (float)MathHelper.Cosh(dist));
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

        public void Move(KeyboardState input, float time)
        {
            float cameraSpeed = _cameraSpeed;

            Vector3 move = Vector3.Zero;

            if (input.IsKeyDown(Keys.W))
            {
                move += _front * cameraSpeed * time;
            }
            if (input.IsKeyDown(Keys.S))
            {
                move -= _front * cameraSpeed * time;
            }
            if (input.IsKeyDown(Keys.A))
            {
                move -= _right * cameraSpeed * time;
            }
            if (input.IsKeyDown(Keys.D))
            {
                move += _right * cameraSpeed * time;
            }
            if (input.IsKeyDown(Keys.Space))
            {
                move += _up * cameraSpeed * time;
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                move -= _up * cameraSpeed * time;
            }

            ReferencePointPosition += move;
        }

        protected override void SetComamnd(string[] args)
        {
            switch (args[0])
            {
                case "fov":
                    Fov = int.Parse(args[1]);
                    break;
                case "curve":
                    if (args[1] == "h")
                        Curve = -1f;
                    else if (args[1] == "s")
                        Curve = 1f;
                    else if (args[1] == "e")
                        Curve = 0f;
                    else
                        Curve = float.Parse(args[1]);
                    break;
                case "speed":
                    _cameraSpeed = float.Parse(args[1]);
                    break;
                case "position":
                    if (args.Length != 4)
                        return;
                    float x = float.Parse(args[1]);
                    float y = float.Parse(args[2]);
                    float z = float.Parse(args[3]);
                    ReferencePointPosition = new Vector3(x, y, z);
                    break;
                default:
                    throw new CommandException($"Property '{args[0]}' not found");
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
                    Console.WriteLine(ReferencePointPosition);
                    break;
                default:
                    throw new CommandException($"Property '{args[0]}' not found");
            }
        }
    }
}