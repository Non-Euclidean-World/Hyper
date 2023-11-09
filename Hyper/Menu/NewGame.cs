using Common;
using Hud.Widgets;
using Hud.Widgets.Colors;
using Hud.Widgets.MultipleChildren;
using Hud.Widgets.SingleChild;
using OpenTK.Mathematics;

namespace Hyper.Menu;

public class NewGame : SingleChildWidget
{
    public event Action<string, GeometryType> Create = null!;

    private readonly HyperInputTextBox _gameNameInput;

    private string[] _saveNames = SaveManager.GetSaves().ToArray();

    public NewGame()
    {
        var size = new Vector2(0.5f, 0.1f);

        _gameNameInput = new HyperInputTextBox(
            placeholderText: "Input Game Name",
            size: size
        );

        Child = new Background(
            child: new Center(
                child: new Column(
                    alignment: Alignment.Greedy,
                    children: new Widget[]
                    {
                        _gameNameInput,
                        new HyperButton(
                            text: "Start Hyper",
                            action: () => StartGame(GeometryType.Hyperbolic),
                            size: size
                            ),
                        new HyperButton(
                            text: "Start Euclidean",
                            action: () => StartGame(GeometryType.Euclidean),
                            size: size
                        ),
                        new HyperButton(
                            text: "Start Spherical",
                            action: () => StartGame(GeometryType.Spherical),
                            size: size
                        ),
                    }
                )
            )
        );
    }

    private void StartGame(GeometryType geometry)
    {
        var name = _gameNameInput.Text;
        var saveNames = SaveManager.GetSaves();
        if (saveNames.Contains(name))
            return;
        Create?.Invoke(name, geometry);
    }

    public override void Render(Context context)
    {
        if (_saveNames.Contains(_gameNameInput.Text))
            _gameNameInput.Color = ColorGetter.GetVector(Color.Red);
        else
            _gameNameInput.Color = ColorGetter.GetVector(Color.White);
        base.Render(context);
    }

    public void Reload()
    {
        _saveNames = SaveManager.GetSaves().ToArray();
    }
}