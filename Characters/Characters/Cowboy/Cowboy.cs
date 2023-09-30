using OpenTK.Mathematics;

namespace Character.Characters.Cowboy;

public class CowboyModel : Model
{
    private static readonly float LocalScale;

    private static readonly Vector3 LocalTranslation;

    static CowboyModel()
    {
        LocalScale = 0.04f;
        LocalTranslation = new Vector3(0, -5, 0);
    }

    public CowboyModel() : base(CowboyResources.Instance, LocalScale, LocalTranslation) { }

    public void Run() => Animator.Play(0);

    public void Idle() => Animator.Reset();
}