using Hyper.HUD;
using Hyper.MarchingCubes;
using Hyper.Meshes;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Hyper
{
    internal class Scene
    {
        internal List<Chunk> Chunks;

        internal List<LightSource> LightSources;

        internal List<Projectile> Projectiles;

        internal Camera Cam;

        internal HUDManager Hud;

        internal const float Scale = 0.1f;

        private Shader _objectShader;

        private Shader _lightSourceShader;

        internal Scene(float aspectRatio)
        {
            Generator generator = new Generator(1);

            Chunks = GetChunks(generator);
            LightSources = GetLightSources(generator);
            Projectiles = new List<Projectile>();
            Cam = GetCamera(generator, aspectRatio);
            Hud = new HUDManager(aspectRatio);

            _objectShader = GetObjectShader();
            _lightSourceShader = GetLightSourceShader();
        }

        public void Render() 
        {
            SetUpObjectShaderParams();

            foreach (var chunk in Chunks)
            {
                chunk.Render(_objectShader, Scale, Cam.ReferencePointPosition);
            }

            foreach (var projectile in Projectiles)
            {
                projectile.Render(_objectShader, Scale, Cam.ReferencePointPosition);
            }

            SetUpLightingShaderParams();

            foreach (var light in LightSources)
            {
                light.Render(_lightSourceShader, Scale, Cam.ReferencePointPosition);
            }

            Hud.Render();
        }

        internal void UpdateProjectiles(float time)
        {
            foreach (var projectile in Projectiles)
            {
                projectile.Update(time);
            }

            Projectiles.RemoveAll(x => x.IsDead);
        }

        internal void UpdateCamera(KeyboardState input, float time, Vector2 mousePosition)
        {
            if (input.IsKeyDown(Keys.D8))
            {
                Cam.Curve = 0f;
            }

            if (input.IsKeyDown(Keys.D9))
            {
                Cam.Curve = 1f;
            }

            if (input.IsKeyDown(Keys.D0))
            {
                Cam.Curve = -1f;
            }

            if (input.IsKeyDown(Keys.Down))
            {
                Cam.Curve -= 0.0001f;
            }

            if (input.IsKeyDown(Keys.Up))
            {
                Cam.Curve += 0.0001f;
            }

            if (input.IsKeyDown(Keys.Tab))
            {
                Console.WriteLine(Cam.Curve);
            }

            Cam.Move(input, time);

            Cam.Turn(mousePosition);
        }

        private void SetUpObjectShaderParams()
        {
            _objectShader.Use();
            _objectShader.SetFloat("curv", Cam.Curve);
            _objectShader.SetFloat("anti", 1.0f);
            _objectShader.SetMatrix4("view", Cam.GetViewMatrix());
            _objectShader.SetMatrix4("projection", Cam.GetProjectionMatrix());
            _objectShader.SetVector3("objectColor", new Vector3(1f, 0.5f, 0.31f));
            _objectShader.SetInt("numLights", LightSources.Count);
            _objectShader.SetVector4("viewPos", Cam.PortEucToCurved(Vector3.UnitY));

            for (int i = 0; i < LightSources.Count; i++)
            {
                _objectShader.SetVector3($"lightColor[{i}]", LightSources[i].Color);
                _objectShader.SetVector4($"lightPos[{i}]", Cam.PortEucToCurved((LightSources[i].Position - Cam.ReferencePointPosition) * Scale));
            }
        }

        private void SetUpLightingShaderParams()
        {
            _lightSourceShader.Use();
            _lightSourceShader.SetFloat("curv", Cam.Curve);
            _lightSourceShader.SetFloat("anti", 1.0f);
            _lightSourceShader.SetMatrix4("view", Cam.GetViewMatrix());
            _lightSourceShader.SetMatrix4("projection", Cam.GetProjectionMatrix());
        }

        private List<Chunk> GetChunks(Generator generator)
        {
            var chunks = new List<Chunk>
            {
                generator.GenerateChunk(new Vector3i(0, 0, 0)),
                generator.GenerateChunk(new Vector3i(Chunk.Size - 1, 0, 0)),
                generator.GenerateChunk(new Vector3i(0, 0, Chunk.Size - 1))
            };

            return chunks;
        }

        private List<LightSource> GetLightSources(Generator generator)
        {
            var lightSources = new List<LightSource> {
                new LightSource(CubeMesh.Vertices, new Vector3(10f, 7f + generator.AvgElevation, 10f), new Vector3(1f, 1f, 1f)),
                new LightSource(CubeMesh.Vertices, new Vector3(4f, 7f + generator.AvgElevation, 4f), new Vector3(0f, 1f, 0.5f)),
            };

            return lightSources;
        }

        private Camera GetCamera(Generator generator, float aspectRatio)
        {
            var camera = new Camera(aspectRatio, 0.01f, 100f, Scale);
            camera.ReferencePointPosition = (5f + generator.AvgElevation) * Vector3.UnitY;

            return camera;
        }

        private Shader GetObjectShader()
        {
            var shaderParams = new (string, ShaderType)[]
            {
                ("Shaders/lighting_shader.vert", ShaderType.VertexShader),
                ("Shaders/lighting_shader.frag", ShaderType.FragmentShader)
            };

            return new Shader(shaderParams);
        }

        private Shader GetLightSourceShader()
        {
            var shaderParams = new (string, ShaderType)[]
            {
                ("Shaders/lighting_shader.vert", ShaderType.VertexShader),
                ("Shaders/light_source_shader.frag", ShaderType.FragmentShader)
            };
            return new Shader(shaderParams);
        }
    }
}
