﻿using OpenTK.Mathematics;

namespace Hyper.Meshes
{
    internal class Projectile : Mesh
    {
        private readonly Vector3 _direction;

        private readonly float _speed;

        private float _lifeTime;

        internal bool IsDead = false;

        internal Projectile(Vertex[] vertices, Vector3 position, Vector3 direction, float speed, float lifeTime) : base(vertices, position)
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
