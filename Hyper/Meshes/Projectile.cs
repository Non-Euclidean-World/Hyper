using OpenTK.Mathematics;

namespace Hyper.Meshes;

internal class Projectile : Mesh
{
    public bool IsDead { get; private set; } = false;

    private readonly Vector3 _direction;

    private readonly float _speed;

    private float _lifeTime;

    public Projectile(Vertex[] vertices, Vector3 position, Vector3 direction, float speed, float lifeTime)
        : base(vertices, position)
    {
        _direction = direction;
        _speed = speed;
        _lifeTime = lifeTime;
    }

    public void Update(float deltaTime)
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
