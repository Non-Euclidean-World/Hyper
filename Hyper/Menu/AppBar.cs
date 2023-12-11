using Hud.Widgets;
using Hud.Widgets.MultipleChildren;
using Hud.Widgets.SingleChild;
using Hyper.Menu.Common;
using OpenTK.Mathematics;

namespace Hyper.Menu;
/// <summary>
/// A bar the lets the user navigate the menu.
/// </summary>
internal class AppBar : SingleChildWidget
{
    public event Action Resume = null!;

    public event Action NewGame = null!;

    public event Action Load = null!;

    public event Action Delete = null!;

    public event Action Controls = null!;

    public event Action Quit = null!;

    private Visibility _resumeWidget = null!;

    public bool ResumeVisible { set => _resumeWidget.Visible = value; }

    public AppBar(bool resumeVisible)
    {
        Child = GetChild(resumeVisible);
    }

    private Widget GetChild(bool resumeVisible)
    {
        var size = new Vector2(0.42f, 0.12f);

        _resumeWidget = new Visibility(
            visible: resumeVisible,
            child: new HyperButton("Resume", () => Resume.Invoke(), size)
        );

        return new Background(
            new Center(
                child: new Column(
                    alignment: Alignment.Greedy,
                    children: new Widget[]
                    {
                        _resumeWidget,
                        new HyperButton("New Game", () => NewGame.Invoke(), size),
                        new HyperButton("Load", () => Load.Invoke(), size),
                        new HyperButton("Delete", () => Delete.Invoke(), size),
                        new HyperButton("Controls", () => Controls.Invoke(), size),
                        new HyperButton("Quit", () => Quit.Invoke(), size)
                    }
                )
            )
        );
    }
}
