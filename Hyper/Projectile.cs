using Hyper.Meshes;
using OpenTK.Mathematics;

namespace Hyper
{
    internal class Projectile : Mesh
    {
        public Vector3 Direction;
        public float Speed;
        public float LifeTime;
        public bool IsDead = false;

        public Projectile(float[] vertices, Vector3 position, Vector3 direction, float speed, float lifeTime) : base(vertices, position)
        {
            Direction = direction;
            Speed = speed;
            LifeTime = lifeTime;
        }

        public void Update(float deltaTime)
        {
            Position += Direction * Speed * deltaTime;
            LifeTime -= deltaTime;

            if (LifeTime < 0)
            {
                IsDead = true;
                Dispose();
            }
        }
    }
}
