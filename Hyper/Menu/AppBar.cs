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

    public AppBar()
    {
        Child = GetChild();
    }

    private Widget GetChild()
    {
        var size = new Vector2(0.42f, 0.12f);

        return new Background(
            new Center(
                child: new Column(
                    alignment: Alignment.Equal,
                    children: new Widget[]
                    {
                        new HyperButton("Resume", () => Resume.Invoke(), size),
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
