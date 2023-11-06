using Hud.Widgets;
using Hud.Widgets.Colors;
using Hud.Widgets.MultipleChildren;
using Hud.Widgets.NoChildren;
using Hud.Widgets.SingleChild;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.Menu;

public class LoadGame : Widget
{
    private const int GamesPerRow = 3;

    private const int TotalGames = 9;
    
    private readonly Widget _child;
    
    public LoadGame()
    {
        _child = GetChild();
    }

    private Widget GetChild()
    {
        string[] saveNames = SaveManager.GetSaves().Take(TotalGames).ToArray();

        return new Background(
            color: Color.Black,
            child: new Column(
                alignment: Alignment.Greedy,
                children: new Widget[]
                {
                    new Padding(
                        size: 0.03f,
                        child: new Center(
                            new TextBox(
                                size: 0.05f,
                                text: "Load Game"
                            )
                        )
                    ),
                    Grid.Build(
                        children: saveNames,
                        GetSaveWidget,
                        GamesPerRow
                    ),
                })
            
        );
    }

    private Widget GetSaveWidget(string name)
    {
        return new ClickDetector(
            action: () => Console.WriteLine($"Load save {name}"),
            child: new Background(
                color: Color.Blue,
                child: new Padding(
                    size: 0.01f,
                    child: new Background(
                        color: Color.Red,
                        child: new Center(
                            child: new TextBox(
                                text: name,
                                size: 0.02f,
                                color: Color.White
                            )
                        )
                    )
                )
            )
        );
    }

    public override Vector2 GetSize()
    {
        return _child.GetSize();
    }

    public override void Render(Context context)
    {
        _child.Render(context);
    }

    public override void Click(Vector2 position)
    {
        _child.Click(position);
    }
    
    public override void KeyboardInput(Keys key)
    {
        _child.KeyboardInput(key);
    }
}