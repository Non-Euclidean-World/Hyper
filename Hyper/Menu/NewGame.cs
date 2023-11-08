using Common;
using Hud.Widgets;
using Hud.Widgets.Colors;
using Hud.Widgets.MultipleChildren;
using Hud.Widgets.NoChildren;
using Hud.Widgets.SingleChild;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.Menu;

public class NewGame : SingleChildWidget
{
    public event Action<string, GeometryType> Create = null!;
    
    private readonly InputTextBox _gameNameInput;

    public NewGame()
    {
        _gameNameInput = new InputTextBox(
            text: "Game Name",
            size: 0.05f);
        
        var size = new Vector2(0.5f, 0.1f);
        
        Child = new Background(
            child: new Center(
                child: new Column(
                    alignment: Alignment.Greedy,
                    children: new Widget[]
                    {
                        new SizeBox(
                            size: size,
                            child: new Padding(
                                size: 0.01f,
                                child: new Background(
                                    color: Color.Secondary,
                                    child: new Padding(
                                        size: 0.01f,
                                        child: new Background(
                                            color: Color.Primary,
                                            child: new Center(_gameNameInput)
                                            )
                                        )
                                    )
                                )
                            ),
                        new HyperButton(
                            text: "Start Hyper", 
                            action: () => Console.WriteLine($"Hyper {_gameNameInput.Text}"),
                            size: size
                            ),
                        new HyperButton(
                            text: "Start Euclidean", 
                            action: () => Console.WriteLine("Euclid"),
                            size: size
                        ),
                        new HyperButton(
                            text: "Start Spherical", 
                            action: () => Console.WriteLine("Sphere"),
                            size: size
                        ),
                    }
                )
            )
        );
    }
}