using Hyper.MarchingCubes;
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

        private LightSource[] _lightSources = null!;

        private List<Projectile> _projectiles = new List<Projectile>();

        private Shader _objectShader = null!;

        private Shader _lightSourceShader = null!;

        private float _scale = 0.1f;

        private Camera _camera = null!;

        private bool _firstMove = true;

        private Vector2 _lastPos;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            StartDebugThreadAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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

            SetUpShaders();

            SetUpScene();

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
            _objectShader.SetInt("numLights", _lightSources.Length);
            _objectShader.SetVector4("viewPos", _camera.PortEucToCurved(Vector3.UnitY));

            for (int i = 0; i < _lightSources.Length; i++)
            {
                _objectShader.SetVector3($"lightColor[{i}]", _lightSources[i].Color);
                _objectShader.SetVector4($"lightPos[{i}]", _camera.PortEucToCurved((_lightSources[i].Position - _camera.ReferencePointPosition) * _scale));
            }

            foreach (var obj in _objects)
            {
                foreach (var mesh in obj.Meshes)
                {
                    mesh.Render(_objectShader, _scale, _camera.ReferencePointPosition);
                }
            }

            foreach (var projectile in _projectiles)
            {
                projectile.Render(_objectShader, _scale, _camera.ReferencePointPosition);
            }

            _lightSourceShader.Use();
            _lightSourceShader.SetFloat("curv", _camera.Curve);
            _lightSourceShader.SetFloat("anti", 1.0f);
            _lightSourceShader.SetMatrix4("view", _camera.GetViewMatrix());
            _lightSourceShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            foreach (var light in _lightSources)
            {
                light.Render(_lightSourceShader, _scale, _camera.ReferencePointPosition);
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
            var time = (float)e.Time;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            UpdateCamera(input, time);

            UpdateProjectiles(time);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Keys.J)
            {
                var projectile = new Projectile(CubeMesh.Vertices, _camera.ReferencePointPosition + 1 / _scale * Vector3.UnitY, _camera.Front, 20f, 5f);
                _projectiles.Add(projectile);
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
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void SetUpShaders()
        {
            var objectShaders = new (string, ShaderType)[]
            {
                ("Shaders/lighting_shader.vert", ShaderType.VertexShader),
                ("Shaders/lighting_shader.frag", ShaderType.FragmentShader)
            };
            _objectShader = new Shader(objectShaders);

            var lightSourceShaders = new (string, ShaderType)[]
            {
                ("Shaders/lighting_shader.vert", ShaderType.VertexShader),
                ("Shaders/light_source_shader.frag", ShaderType.FragmentShader)
            };
            _lightSourceShader = new Shader(lightSourceShaders);
        }

        private void SetUpScene()
        {
            Generator generator = new Generator(0, 64);
            _objects = generator.GenerateWrold();

            _lightSources = new LightSource[] {
                new LightSource(CubeMesh.Vertices, new Vector3(20f, 10f, 20f), new Vector3(1f, 1f, 1f)),
                new LightSource(CubeMesh.Vertices, new Vector3(40f, 10f, 40f), new Vector3(0f, 1f, 0.5f)),
            };

            _camera = new Camera(Size.X / (float)Size.Y, 0.01f, 100f);
        }

        private void UpdateCamera(KeyboardState input, float time)
        {
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

            _camera.Move(input, time);

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

        private void UpdateProjectiles(float time)
        {
            foreach (var projectile in _projectiles)
            {
                projectile.Update(time);
            }

            _projectiles.RemoveAll(x => x.IsDead);
        }
    }
}