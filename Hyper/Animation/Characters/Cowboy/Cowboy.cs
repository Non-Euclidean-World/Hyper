using OpenTK.Mathematics;

namespace Hyper.Animation.Characters.Cowboy;

internal class Cowboy : Character
{
    public Cowboy(Vector3 position, float scale) : base(position, scale)
    {
        var model = Path.GetFullPath("Animation/Characters/Cowboy/Resources/model.dae");
        var texture = Path.GetFullPath("Animation/Characters/Cowboy/Resources/texture.png");
        Model = new Model(model, texture);
    }

    public void Run() => Model.Animator.Play(0);

    public void Idle() => Model.Animator.Reset();
}