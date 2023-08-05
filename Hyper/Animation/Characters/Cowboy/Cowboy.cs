using OpenTK.Mathematics;

namespace Hyper.Animation.Characters.Cowboy;

internal class Cowboy : Character
{
    public Cowboy()
    {
        var model = Path.GetFullPath("Resources/model.dae");
        var texture = Path.GetFullPath("Resources/model.png");
        Model = new Model(model, texture);
    }

    public void Run() => Model.Animator.Restart(0);

    public void Idle() => Model.Animator.Reset();
}