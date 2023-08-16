
using OpenTK.Mathematics;

namespace Hyper.Animation.Characters.Cowboy;

internal class Cowboy : CharacterModel
{
    private static Model CreateModel()
    {
        string model = Path.GetFullPath("Animation/Characters/Cowboy/Resources/model.dae");
        string texture = Path.GetFullPath("Animation/Characters/Cowboy/Resources/texture.png");

        return new Model(model, texture);
    }

    public static Vector3 LocalTranslation => new(0, -5f, 0); // make sure the middle point is in (0, 0, 0)

    public Cowboy(float scale) : base(scale, CreateModel()) { }

    public void Run() => Model.Animator.Play(0);

    public void Idle() => Model.Animator.Reset();
}