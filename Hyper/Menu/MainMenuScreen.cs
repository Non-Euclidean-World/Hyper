using Hud.Widgets;
using Hud.Widgets.Colors;
using Hud.Widgets.MultipleChildren;
using Hud.Widgets.NoChildren;
using Hud.Widgets.SingleChild;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.Menu;
internal class MainMenuScreen : Widget
{
    private readonly Widget _child;
    
    public event Action Resume;
    
    public event Action Load;
    
    public event Action Quit;

    public MainMenuScreen()
    {
        _child = GetChild();
    }

    private Widget GetChild()
    {
        return new Center(
            child: new Column(
                alignment: Alignment.Equal,
                children: new Widget[]
                {
                    CreateButton("Resume", () => Resume.Invoke()),
                    CreateButton("Load", () => Load.Invoke()),
                    CreateButton("Quit", () => Quit.Invoke())
                }
            )
        );
    }

    private Widget CreateButton(string text, Action action)
    {
        return new Padding(
            size: 0.01f,
            child: new Background(
                color: Color.White,
                child: new Padding(
                    size: 0.01f,
                    child: new Button(
                        size: new Vector2(0.4f, 0.1f),
                        action: () => action?.Invoke(),
                        text: text
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
