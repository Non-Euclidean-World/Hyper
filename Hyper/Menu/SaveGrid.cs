using Hud.Widgets;
using Hud.Widgets.Colors;
using Hud.Widgets.MultipleChildren;
using Hud.Widgets.NoChildren;
using Hud.Widgets.SingleChild;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.Menu;

public class SaveGrid : Widget
{
    private const int GamesPerRow = 3;

    private const int TotalGames = 9;
    
    private Widget _child;

    public event Action<string> OnSelected;

    private TextBox _titleTextBox;
    
    public string Title
    {
        get => _titleTextBox.Text;
        set => _titleTextBox.Text = value;
    }

    public SaveGrid()
    {
        _child = GetChild("Load Game");
    }
    
    public void Reload()
    {
        _child = GetChild(Title);
    }

    private Widget GetChild(string title)
    {
        string[] saveNames = SaveManager.GetSaves().Take(TotalGames).ToArray();
        saveNames = saveNames.Concat(Enumerable.Repeat(string.Empty, TotalGames - saveNames.Length)).ToArray();
        
        _titleTextBox = new TextBox(title, 0.05f);

        return new Background(
            color: Color.Background,
            child: new Column(
                alignment: Alignment.Greedy,
                children: new Widget[]
                {
                    new Padding(
                        size: 0.03f,
                        child: new Center(
                            _titleTextBox
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
        return  new Padding(
            size: 0.02f,
            child: new ClickDetector(
                action: () =>
                {
                    if (name.Length == 0)
                        return;
                    OnSelected?.Invoke(name);
                },
                child: new Background(
                    color: Color.Secondary,
                    child: new Padding(
                        size: 0.01f,
                        child: new Background(
                            color: Color.Primary,
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