using OpenTK.Mathematics;

namespace Hyper.Meshes
{
    internal class Projectile : Mesh
    {
        private Vector3 _direction;

        private float _speed;

        private float _lifeTime;

        internal bool IsDead = false;

        internal Projectile(float[] vertices, Vector3 position, Vector3 direction, float speed, float lifeTime) : base(vertices, position)
        {
            _direction = direction;
            _speed = speed;
            _lifeTime = lifeTime;
        }

        internal void Update(float deltaTime)
        {
            Position += _direction * _speed * deltaTime;
            _lifeTime -= deltaTime;

            if (_lifeTime < 0)
            {
                IsDead = true;
                Dispose();
            }
        }
    }
}
