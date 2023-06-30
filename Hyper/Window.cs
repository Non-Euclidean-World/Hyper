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

        private Shader _shader = null!;

        private Texture _texture = null!;

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

            var shaders = new (string, ShaderType)[]
            {
                ("Shaders/shader.vert", ShaderType.VertexShader),
                ("Shaders/shader.frag", ShaderType.FragmentShader)
            };

            _shader = new Shader(shaders);
            _shader.Use();

            _objects = GenerateObjects(() => SceneGenerators.GenerateFlat(20));

            // Position attribute
            var vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            // Texture coordinate attribute
            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            _texture = Texture.LoadFromFile("Resources/container.png");
            _texture.Use(TextureUnit.Texture0);

            _shader.SetInt("texture0", 0);

            _camera = new Camera(Size.X / (float)Size.Y, 0.01f, 100f);

            CursorState = CursorState.Grabbed;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindVertexArray(_objects[0].Meshes[0].VaoId); // we only have 1 VAO but will have to change this

            _texture.Use(TextureUnit.Texture0);

            _shader.Use();

            _shader.SetFloat("curv", _camera.Curve);
            _shader.SetFloat("anti", 1.0f);
            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            foreach (var obj in _objects)
            {
                foreach (var mesh in obj.Meshes)
                {
                    var model = Matrix4.CreateTranslation((mesh.Position - _camera.Position) * _scale);
                    var scale = Matrix4.CreateScale(_scale);
                    _shader.SetMatrix4("model", scale * model);

                    GL.DrawElements(PrimitiveType.Triangles, mesh.numberOfIndices, DrawElementsType.UnsignedInt, 0);
                }
            }


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
            var positions = positionsGenerator();
            var object3d = new Object3D();
            foreach (var position in positions)
            {
                object3d.Meshes.Add(CubeMesh.Create(1f, position));
            }

            return new List<Object3D> { object3d };
        }
    }
}
