using Hyper.Meshes;
using NLog;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper
{
    internal class Window : GameWindow
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private CancellationTokenSource _debugCancellationTokenSource = null!;

        private Scene _scene = null!;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            StartDebugThreadAsync().ConfigureAwait(false);
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
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            _scene = new Scene(Size.X / (float)Size.Y);

            CursorState = CursorState.Grabbed;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _scene.Render();

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

            _scene.UpdateCamera(input, time, new Vector2(MouseState.X, MouseState.Y));

            _scene.UpdateProjectiles(time);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButton.Middle)
            {
                var projectile = new Projectile(CubeMesh.Vertices, _scene.Camera.ReferencePointPosition + 1 / Scene.Scale * Vector3.UnitY, _scene.Camera.Front, 20f, 5f);
                _scene.Projectiles.Add(projectile);
            }

            // These 2 do not work on chunk borders.
            if (e.Button == MouseButton.Left)
            {
                var position = _scene.Camera.ReferencePointPosition;

                foreach (var chunk in _scene.Chunks)
                {
                    if (chunk.Mine(position, 1f)) return;
                }
            }

            if (e.Button == MouseButton.Right)
            {
                var position = _scene.Camera.ReferencePointPosition;

                foreach (var chunk in _scene.Chunks)
                {
                    if (chunk.Build(position, 1f)) return;
                }
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _scene.Camera.Fov -= e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            _scene.Camera.AspectRatio = Size.X / (float)Size.Y;
            _scene.Hud.AspectRatio = Size.X / (float)Size.Y;
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

                Logger.Info($"[Command]{command}");

                try
                {
                    var args = command.Split(' ');
                    var key = args[0];
                    args = args.Skip(1).ToArray();

                    switch (key)
                    {
                        case "camera":
                            _scene.Camera.Command(args);
                            break;
                        case "hud":
                            _scene.Hud.Command(args);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}