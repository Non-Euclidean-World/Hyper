using Common;

namespace Hud;

public interface IHudElement
{
    public bool Visible { get; set; }

    public void Render(Shader shader);
}
