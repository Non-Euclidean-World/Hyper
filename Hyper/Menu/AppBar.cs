using Hud.Widgets;
using Hud.Widgets.Colors;
using Hud.Widgets.MultipleChildren;
using Hud.Widgets.NoChildren;
using Hud.Widgets.SingleChild;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.Menu;
internal class AppBar : SingleChildWidget
{
    public event Action Resume;
    
    public event Action NewGame;
    
    public event Action Load;

    public event Action Delete;
    
    public event Action Quit;

    public AppBar()
    {
        Child = GetChild();
    }

    private Widget GetChild()
    {
        var size = new Vector2(0.42f, 0.12f);
        
        return new Center(
            child: new Column(
                alignment: Alignment.Equal,
                children: new Widget[]
                {
                    new HyperButton("Resume", () => Resume.Invoke(), size),
                    new HyperButton("New Game", () => NewGame.Invoke(), size),
                    new HyperButton("Load", () => Load.Invoke(), size),
                    new HyperButton("Delete", () => Delete.Invoke(), size),
                    new HyperButton("Quit", () => Quit.Invoke(), size)
                }
            )
        );
    }
}
