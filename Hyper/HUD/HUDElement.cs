using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.HUD
{
    internal abstract class HUDElement
    {
        public bool Visible { get; set; } = true;

        protected int _vaoId;

        protected Vector2 _position;

        protected float _size;

        public HUDElement(Vector2 position, float size)
        {
            _position = position;
            _size = size;
        }

        public abstract void Render(Shader shader);
    }
}
