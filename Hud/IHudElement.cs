using Common;

namespace Hud;

public interface IHudElement : IDisposable
{
    public bool Visible { get; set; }

    public void Render(Shader shader);
}
