using Common;

namespace Hyper.HUD;

internal interface IHudElement
{
    public bool Visible { get; set; }

    public void Render(Shader shader);
}
