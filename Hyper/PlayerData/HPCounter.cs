using Common;
using Hud;
using Hud.Widgets.Colors;
using OpenTK.Mathematics;

namespace Hyper.PlayerData;
internal class HPCounter : IHudElement
{
    public bool Visible { get; set; } = true;

    private const float Size = 0.04f;

    private readonly IWindowHelper _windowHelper;

    private readonly Player _player;

    public HPCounter(IWindowHelper windowHelper, Player player)
    {
        _windowHelper = windowHelper;
        _player = player;
    }

    public void Render(Shader shader)
    {
        shader.SetVector4("color", GetColor());

        Printer.RenderStringTopRight(shader, GetHPStatusString(), Size, 0, 0.47f);
    }

    private string GetHPStatusString()
    {
        return $"HP: {_player.HP}/{_player.MaxHP}";
    }

    private Vector4 GetColor()
    {
        if (_player.HP < _player.MaxHP / 3f)
            return ColorGetter.GetVector(Color.Red);
        else
            return Vector4.One;
    }

    public void Dispose()
    {
        // nothing to dispose
    }
}
