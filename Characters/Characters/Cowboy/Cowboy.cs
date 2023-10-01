using OpenTK.Mathematics;

namespace Character.Characters.Cowboy;

public class CowboyModel : Model
{
    private static readonly string ModelPath;

    private static readonly string TexturePath;

    private static readonly float LocalScale;

    private static readonly Vector3 LocalTranslation;

    static CowboyModel()
    {
        ModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Cowboy/model.dae");
        TexturePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Cowboy/texture.png");
        LocalScale = 0.5f;
        LocalTranslation = new Vector3(0, -5, 0);
    }

    public CowboyModel() : base(ModelPath, TexturePath, LocalScale, LocalTranslation) { }

    public void Run() => Animator.Play(0);

    public void Idle() => Animator.Reset();
}