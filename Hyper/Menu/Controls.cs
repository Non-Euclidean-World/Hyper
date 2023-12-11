using Hud.Widgets;
using Hud.Widgets.Colors;
using Hud.Widgets.MultipleChildren;
using Hud.Widgets.SingleChild;
using Hyper.Menu.Common;
using OpenTK.Mathematics;

namespace Hyper.Menu;
/// <summary>
/// The controls menu. It shows the controls of the game.
/// </summary>
public class Controls : SingleChildWidget
{
    private static readonly Dictionary<string, string> KeyMap = new()
    {
        {"W", "Move Forward"},
        {"S", "Move Back"},
        {"A", "Move Left"},
        {"D", "Move Right"},
        {"Shift", "Sprint"},
        {"Space", "Jump"},
        {"Left Mouse", "Use Item"},
        {"Right Mouse", "Alt Use Item"},
        {"Esc", "Show/Hide Menu"},
        {"Tab", "Switch camera"},
        {"Scroll", "Change FOV"},
        {"0-9", "Select Item"},
        {"C", "Enter Car"},
        {"F", "Flip Car"},
        {"L", "Leave Car"},
        {"Y", "Toggle Flashlight"},
    };

    public Controls()
    {
        Child = GetChild();
    }

    private Widget GetChild()
    {
        return new Center(
            child: new Background(
                color: Color.Background,
                child: new Padding(
                    size: 0.02f,
                    child: Grid.Build(
                        alignment: Alignment.Greedy,
                        numberOfChildrenInARow: 2,
                        children: KeyMap,
                        widgetMaker: GetKeyWidget
                    )
                )
            )
        );
    }

    private Widget GetKeyWidget(KeyValuePair<string, string> keyMapping)
    {
        const float width = 0.6f;
        const float height = 0.1f;
        var padding = new Vector2(0.03f);

        return new Padding(
            size: 0.01f,
            child: new SizeBox(
                size: new Vector2(width, height),
                child: new Background(
                    color: Color.Primary,
                    child: new Center(
                        child: new Row(
                            alignment: Alignment.Greedy,
                            children: new Widget[]
                            {
                                new HyperText(keyMapping.Key, new Vector2(width / 2, height) - padding),
                                new HyperText(keyMapping.Value, new Vector2(width / 2, height) - padding)
                            }
                        )
                    )
                )
            )
        );
    }
}