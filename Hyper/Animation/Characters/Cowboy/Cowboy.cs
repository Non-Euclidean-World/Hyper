using OpenTK.Mathematics;

namespace Hyper.Animation.Characters.Cowboy;

internal class Cowboy : Character
{
    private static Model GetModel()
    {
        string model = Path.GetFullPath("Animation/Characters/Cowboy/Resources/model.dae");
        string texture = Path.GetFullPath("Animation/Characters/Cowboy/Resources/texture.png");

        return new Model(model, texture);
    }

    public Cowboy(Vector3 position, float scale) : base(position, scale, GetModel())
    {
    }

    public Cowboy() : this(Vector3.Zero, 0.04f) { }

    public void Run() => Model.Animator.Play(0);

    public void Idle() => Model.Animator.Reset();
}