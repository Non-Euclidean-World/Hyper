﻿using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.HUD
{
    internal abstract class HUDElement
    {
        public bool Visible { get; set; } = true;

        protected int _vaoId;

        protected Vector2 _position;

        protected float _size;

        /// <summary>
        /// This constructor is used for HUD elements that don't use textures.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="vertices">vertices should be a 1d array of the form: 2 floats for position, 3 floats for color, 2 floats for texture coordinates.</param>
        public HUDElement(Vector2 position, float size, float[] vertices)
        {
            _position = position;
            _size = size;

            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            int vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 7 * sizeof(float), 5 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            _vaoId = vao;
        }

        public abstract void Render(Shader shader);
    }
}
