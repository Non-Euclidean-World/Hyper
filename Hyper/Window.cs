using Hyper.Meshes;
using NLog;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper
{
    public class Window : GameWindow
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private CancellationTokenSource _debugCancellationTokenSource = null!;

        private List<Object3D> _objects = null!;

        private Mesh _lightSource = null!;

        private Shader _objectShader = null!;

        private Shader _lightSourceShader = null!;

        private float _scale = 0.1f;

        private Camera _camera = null!;

        private bool _firstMove = true;

        private Vector2 _lastPos;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            StartDebugThreadAsync();
        }

        public override void Close()
        {
            StopDebugThread();
            base.Close();
            LogManager.Flush();
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0f, 0f, 0f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            var objectShaders = new (string, ShaderType)[]
            {
                ("Shaders/lighting_shader.vert", ShaderType.VertexShader),
                ("Shaders/lighting_shader.frag", ShaderType.FragmentShader)
            };
            _objectShader = new Shader(objectShaders);

            _objects = GenerateObjects(() => SceneGenerators.GenerateFlat(20));

            var lightSourceShaders = new (string, ShaderType)[]
            {
                ("Shaders/lighting_shader.vert", ShaderType.VertexShader),
                ("Shaders/light_source_shader.frag", ShaderType.FragmentShader)
            };
            _lightSourceShader = new Shader(lightSourceShaders);
            _lightSource = GenerateObjects(new Vector3[] { new(2f, 4f, 2f) })[0].Meshes[0];

            _camera = new Camera(Size.X / (float)Size.Y, 0.01f, 100f);

            CursorState = CursorState.Grabbed;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _objectShader.Use();
            _objectShader.SetFloat("curv", _camera.Curve);
            _objectShader.SetFloat("anti", 1.0f);
            _objectShader.SetMatrix4("view", _camera.GetViewMatrix());
            _objectShader.SetMatrix4("projection", _camera.GetProjectionMatrix());
            _objectShader.SetVector3("objectColor", new Vector3(1f, 0.5f, 0.31f));
            _objectShader.SetVector3("lightColor", new Vector3(1f, 1f, 1f));
            _objectShader.SetVector4("lightPos", _camera.PortEucToCurved((_lightSource.Position - _camera.Position) * _scale));
            _objectShader.SetVector4("viewPos", _camera.PortEucToCurved(Vector3.UnitY));

            foreach (var obj in _objects)
            {
                foreach (var mesh in obj.Meshes)
                {
                    var model = Matrix4.CreateTranslation((mesh.Position - _camera.Position) * _scale);
                    var scale = Matrix4.CreateScale(_scale);
                    _objectShader.SetMatrix4("model", scale * model);

                    GL.BindVertexArray(mesh.VaoId);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                }
            }


            _lightSourceShader.Use();
            _lightSourceShader.SetFloat("curv", _camera.Curve);
            _lightSourceShader.SetFloat("anti", 1.0f);
            _lightSourceShader.SetMatrix4("view", _camera.GetViewMatrix());
            _lightSourceShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            GL.BindVertexArray(_lightSource.VaoId);
            var modelLS = Matrix4.CreateTranslation((_lightSource.Position - _camera.Position) * _scale);
            var scaleLS = Matrix4.CreateScale(_scale);
            _lightSourceShader.SetMatrix4("model", scaleLS * modelLS);

            GL.BindVertexArray(_lightSource.VaoId);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused)
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            if (input.IsKeyDown(Keys.D8))
            {
                _camera.Curve = 0f;
            }

            if (input.IsKeyDown(Keys.D9))
            {
                _camera.Curve = 1f;
            }

            if (input.IsKeyDown(Keys.D0))
            {
                _camera.Curve = -1f;
            }

            if (input.IsKeyDown(Keys.Down))
            {
                _camera.Curve -= 0.0001f;
            }

            if (input.IsKeyDown(Keys.Up))
            {
                _camera.Curve += 0.0001f;
            }

            if (input.IsKeyDown(Keys.Tab))
            {
                Console.WriteLine(_camera.Curve);
            }

            const float sensitivity = 0.2f;

            _camera.Move(input, (float)e.Time);

            var mouse = MouseState;

            if (_firstMove)
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _camera.Fov -= e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            _camera.AspectRatio = Size.X / (float)Size.Y;
        }

        private async Task StartDebugThreadAsync()
        {
            _debugCancellationTokenSource = new CancellationTokenSource();
            await Task.Run(() => Command(_debugCancellationTokenSource.Token));
        }

        private void StopDebugThread()
        {
            _debugCancellationTokenSource.Cancel();
        }

        private void Command(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                string? command = Console.ReadLine();
                if (command == null)
                    return;

                _logger.Info($"[Command]{command}");

                try
                {
                    var args = command.Split(' ');
                    var key = args[0];
                    args = args.Skip(1).ToArray();

                    switch (key)
                    {
                        case "camera":
                            _camera.Command(args);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }
        }

        private static List<Object3D> GenerateObjects(Func<Vector3[]> positionsGenerator)
        {
            return GenerateObjects(positionsGenerator());
        }

        private static List<Object3D> GenerateObjects(Vector3[] positions)
        {
            var object3d = new Object3D();
            foreach (var position in positions)
            {
                object3d.Meshes.Add(CubeMesh.Create(position));
            }

            return new List<Object3D> { object3d };
        }
    }
}
