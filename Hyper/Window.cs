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

        private CancellationTokenSource _debugCancellationTokenSource;

        //private readonly float[] _vertices =
        //{
        //    // Position         Texture coordinates
        //    0.5f,  0.5f,  0.5f, 1.0f, 1.0f, // top right, front
        //    0.5f, -0.5f,  0.5f, 1.0f, 0.0f, // bottom right, front
        //    -0.5f, -0.5f,  0.5f, 0.0f, 0.0f, // bottom left, front
        //    -0.5f,  0.5f,  0.5f, 0.0f, 1.0f, // top left, front

        //    0.5f,  0.5f, -0.5f, 1.0f, 1.0f, // top right, back
        //    -0.5f,  0.5f, -0.5f, 0.0f, 1.0f, // top left, back
        //    -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, // bottom left, back
        //    0.5f, -0.5f, -0.5f, 1.0f, 0.0f, // bottom right, back

        //    0.5f,  0.5f,  0.5f, 1.0f, 1.0f, // top right, top
        //    -0.5f,  0.5f,  0.5f, 0.0f, 1.0f, // top left, top
        //    -0.5f,  0.5f, -0.5f, 0.0f, 0.0f, // bottom left, top
        //    0.5f,  0.5f, -0.5f, 1.0f, 0.0f, // bottom right, top

        //    0.5f, -0.5f,  0.5f, 1.0f, 1.0f, // top right, bottom
        //    -0.5f, -0.5f,  0.5f, 0.0f, 1.0f, // top left, bottom
        //    -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, // bottom left, bottom
        //    0.5f, -0.5f, -0.5f, 1.0f, 0.0f, // bottom right, bottom

        //    0.5f,  0.5f,  0.5f, 1.0f, 1.0f, // top right, right
        //    0.5f, -0.5f,  0.5f, 0.0f, 1.0f, // top left, right
        //    0.5f, -0.5f, -0.5f, 0.0f, 0.0f, // bottom left, right
        //    0.5f,  0.5f, -0.5f, 1.0f, 0.0f, // bottom right, right

        //    -0.5f,  0.5f,  0.5f, 1.0f, 1.0f, // top right, left
        //    -0.5f, -0.5f,  0.5f, 0.0f, 1.0f, // top left, left
        //    -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, // bottom left, left
        //    -0.5f,  0.5f, -0.5f, 1.0f, 0.0f  // bottom right, left
        //};

        //private readonly uint[] _indices =
        //{
        //    0,  1,  3,  1,  2,  3,  // Front face
        //    4,  5,  7,  5,  6,  7,  // Back face
        //    8,  9, 11,  9, 10, 11,  // Top face
        //    12, 13, 15, 13, 14, 15,  // Bottom face
        //    16, 17, 19, 17, 18, 19,  // Right face
        //    20, 21, 23, 21, 22, 23   // Left face
        //};

        private List<Object3D> _objects;

        private Shader _shader;

        private Texture _texture;

        //private Texture _texture2;

        // The view and projection matrices have been removed as we don't need them here anymore.
        // They can now be found in the new camera class.

        // We need an instance of the new camera class so it can manage the view and projection matrix code.
        // We also need a boolean set to true to detect whether or not the mouse has been moved for the first time.
        // Finally, we add the last position of the mouse so we can calculate the mouse offset easily.
        private Camera _camera;

        private bool _firstMove = true;

        private Vector2 _lastPos;

        private double _time;

        private Vector3 _cameraPosition = Vector3.Zero;

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

            _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            _shader.Use();

            _objects = GenerateObjects(() => GenerateFlat(10));

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

            /*            _texture2 = Texture.LoadFromFile("Resources/awesomeface.png");
                        _texture2.Use(TextureUnit.Texture1);*/

            _shader.SetInt("texture0", 0);
            //_shader.SetInt("texture1", 1);
            //GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
            // We initialize the camera so that it is 3 units back from where the rectangle is.
            // We also give it the proper aspect ratio.
            _camera = new Camera(Vector3.UnitY, Size.X / (float)Size.Y);

            // We make the mouse cursor invisible and captured so we can have proper FPS-camera movement.
            CursorState = CursorState.Grabbed;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            _time += 4.0 * e.Time;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindVertexArray(_objects[0].Meshes[0].VaoId); // we only have 1 VAO but will have to change this

            _texture.Use(TextureUnit.Texture0);
            //_texture2.Use(TextureUnit.Texture1);
            _shader.Use();

            _shader.SetFloat("curv", _camera.Curve);
            _shader.SetFloat("anti", 1.0f);
            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            foreach (var obj in _objects)
            {
                foreach (var mesh in obj.Meshes)
                {
                    //GL.BindVertexArray(mesh.VaoId); // we only have 1 VAO but will have to change this

                    var model = Matrix4.CreateTranslation(mesh.Position - _cameraPosition);

                    var scale = Matrix4.CreateScale(1f); // will need to change scale
                    _shader.SetMatrix4("model", scale * model);

                    GL.DrawElements(PrimitiveType.Triangles, mesh.numberOfIndices, DrawElementsType.UnsignedInt, 0);
                }
            }


            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused) // Check to see if the window is focused
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

            _cameraPosition += _camera.Move(input, (float)e.Time);

            // Get the mouse state
            var mouse = MouseState;

            if (_firstMove) // This bool variable is initially set to true.
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
            }
        }

        // In the mouse wheel function, we manage all the zooming of the camera.
        // This is simply done by changing the FOV of the camera.
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _camera.Fov -= e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            // We need to update the aspect ratio once the window has been resized.
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
                var command = Console.ReadLine();
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

        private Vector3[] GenerateCubePositions(int nInDim)
        {
            float div = 5.0f;

            Vector3[] positions = new Vector3[nInDim * nInDim * nInDim];
            for (int i = 0; i < nInDim; i++)
                for (int j = 0; j < nInDim; j++)
                    for (int k = 0; k < nInDim; k++)
                    {
                        float x = 2 * (i - nInDim / 2f) / div;
                        float y = 2 * (j - nInDim / 2f) / div;
                        float z = 2 * (k - nInDim / 2f) / div;
                        positions[i * nInDim * nInDim + j * nInDim + k] = new Vector3(x, y, z);
                    }

            return positions;
        }

        private Vector3[] GenerateFlat(int n)
        {
            List<Vector3> positions = new List<Vector3>();
            Random random = new Random(0);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    int maxHeight = random.Next(1, 4);
                    for (int k = 0; k < maxHeight; k++)
                    {
                        float x = (i - n / 2f);
                        float y = k;
                        float z = (j - n / 2f);
                        positions.Add(new Vector3(x, y, z));
                    }
                }
            }

            return positions.ToArray();
        }

        private Vector3[] GenerateColumn(int n)
        {
            List<Vector3> positions = new List<Vector3>();


            for (int k = 0; k < n; k++)
            {
                float y = k;
                positions.Add(new Vector3(0, y, 0));
            }


            return positions.ToArray();
        }

        private Vector3[] CreateSphereOfCubes(int cubeCount)
        {
            Vector3[] cubes = new Vector3[cubeCount];
            Random random = new Random();

            for (int i = 0; i < cubeCount; i++)
            {
                // Create a new cube
                cubes[i] = new Vector3();

                // Generate random theta and phi angles
                float theta = (float)(random.NextDouble() * 2 * Math.PI);
                float phi = (float)(random.NextDouble() * Math.PI);

                // Convert spherical coordinates to Cartesian coordinates
                float x = (float)(Math.Sin(phi) * Math.Cos(theta));
                float y = (float)(Math.Sin(phi) * Math.Sin(theta));
                float z = (float)(Math.Cos(phi));

                // Set the cube's position
                cubes[i] = new Vector3(x, y, z) * 0.8f;
            }

            return cubes;
        }

        private static void CreateUVSphere(float radius, int numLongitudeLines, int numLatitudeLines, out Vector3[] vertices, out int[] indices)
        {
            List<Vector3> verticesList = new List<Vector3>();
            List<int> indicesList = new List<int>();

            // Generate vertices
            for (int lat = 0; lat <= numLatitudeLines; ++lat)
            {
                float theta = lat * MathF.PI / numLatitudeLines;
                float sinTheta = MathF.Sin(theta);
                float cosTheta = MathF.Cos(theta);

                for (int lon = 0; lon <= numLongitudeLines; ++lon)
                {
                    float phi = lon * 2 * MathF.PI / numLongitudeLines;
                    float sinPhi = MathF.Sin(phi);
                    float cosPhi = MathF.Cos(phi);

                    float x = cosPhi * sinTheta;
                    float y = cosTheta;
                    float z = sinPhi * sinTheta;

                    verticesList.Add(new Vector3(x * radius, y * radius, z * radius));
                }
            }

            // Generate indices
            for (int lat = 0; lat < numLatitudeLines; ++lat)
            {
                for (int lon = 0; lon < numLongitudeLines; ++lon)
                {
                    int first = (lat * (numLongitudeLines + 1)) + lon;
                    int second = first + numLongitudeLines + 1;

                    indicesList.Add(first);
                    indicesList.Add(second);
                    indicesList.Add(first + 1);

                    indicesList.Add(second);
                    indicesList.Add(second + 1);
                    indicesList.Add(first + 1);
                }
            }

            vertices = verticesList.ToArray();
            indices = indicesList.ToArray();
        }

        private List<Object3D> GenerateObjects(Func<Vector3[]> generator)
        {
            var positions = generator();
            var object3d = new Object3D();
            foreach (var position in positions)
            {
                object3d.Meshes.Add(CubeMesh.Create(1f, position));
            }

            return new List<Object3D> { object3d };
        }
    }
}
