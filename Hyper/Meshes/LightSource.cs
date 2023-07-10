﻿using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.Meshes
{
    internal class LightSource : Mesh
    {
        public Vector3 Color;

        public LightSource(float[] vertices, Vector3 position, Vector3 color) : base(vertices, position)
        {
            Color = color;
        }

        public override void Render(Shader shader, float scale, Vector3 cameraPosition)
        {
            var modelLS = Matrix4.CreateTranslation((Position - cameraPosition) * scale);
            var scaleLS = Matrix4.CreateScale(scale);
            shader.SetMatrix4("model", scaleLS * modelLS);
            shader.SetVector3("color", Color);

            GL.BindVertexArray(VaoId);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }
    }
}