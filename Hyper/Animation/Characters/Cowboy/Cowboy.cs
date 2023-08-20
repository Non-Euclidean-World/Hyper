using OpenTK.Mathematics;

namespace Hyper.Animation.Characters.Cowboy;

internal class CowboyModel : Model
{
    private static readonly string ModelPath;

    private static readonly string TexturePath;

    private static readonly float LocalScale;

    private static readonly Vector3 LocalTranslation;

    static CowboyModel()
    {
        ModelPath = Path.GetFullPath("Animation/Characters/Cowboy/Resources/model.dae");
        TexturePath = Path.GetFullPath("Animation/Characters/Cowboy/Resources/texture.png");
        LocalScale = 0.04f;
        LocalTranslation = new Vector3(0, -5, 0);
    }

    public CowboyModel() : base(ModelPath, TexturePath, LocalScale, LocalTranslation)
    { }

    public void Run() => Animator.Play(0);

    public void Idle() => Animator.Reset();
}