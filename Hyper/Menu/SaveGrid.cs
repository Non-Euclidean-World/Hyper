using Hud.Widgets;
using Hud.Widgets.Colors;
using Hud.Widgets.MultipleChildren;
using Hud.Widgets.NoChildren;
using Hud.Widgets.SingleChild;

namespace Hyper.Menu;

public class SaveGrid : SingleChildWidget
{
    private const int GamesPerRow = 3;

    private const int TotalGames = 9;

    public event Action<string> OnSelected;

    private Text _titleTextBox;

    public string Title
    {
        get => _titleTextBox.Content;
        set => _titleTextBox.Content = value;
    }

    public SaveGrid()
    {
        Child = GetChild("Load Game");
    }

    public void Reload()
    {
        Child = GetChild(Title);
    }

    private Widget GetChild(string title)
    {
        string[] saveNames = SaveManager.GetSaves().Take(TotalGames).ToArray();
        saveNames = saveNames.Concat(Enumerable.Repeat(string.Empty, TotalGames - saveNames.Length)).ToArray();

        _titleTextBox = new Text(title, 0.05f);

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
        return new Padding(
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
                                child: new Text(
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
}