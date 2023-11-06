using Hud.Widgets;
using Hud.Widgets.Colors;
using Hud.Widgets.MultipleChildren;
using Hud.Widgets.NoChildren;
using Hud.Widgets.SingleChild;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.Menu;

public class SavedGames : Widget
{
    private const int GamesPerRow = 3;
    
    private readonly Widget _child;
    
    public SavedGames()
    {
        string[] saveNames = new[]
        {
            "save1", "save2", "save3", "save4", "save5", "save6", "save7", "save8", "save9",
        };

        _child = new Background(
            color: Color.Black,
            child: Grid.Build(
                children: saveNames,
                GetSave,
                GamesPerRow
            )
        );
    }

    private Widget GetSave(string name)
    {
        return new Background(
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