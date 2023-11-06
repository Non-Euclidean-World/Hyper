using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hud.Widgets;
using Hud.Widgets.MultipleChildren;
using Hud.Widgets.NoChildren;
using Hud.Widgets.SingleChild;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using static BepuPhysics.Collidables.CompoundBuilder;

namespace Hyper.Menu;
internal class MainMenuScreen : Widget
{
    private readonly Widget _child;

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
                    new Button(
                        size: new Vector2(0.8f, 0.2f),
                        action: () => Console.WriteLine("Resume"),
                        text: "Resume"
                        ),
                    new Button(
                        size: new Vector2(0.8f, 0.2f),
                        action: () => Console.WriteLine("Load"),
                        text: "Load"
                        ),
                    new Button(
                        size: new Vector2(0.8f, 0.2f),
                        action: () => Console.WriteLine("Quit"),
                        text: "Quit"
                        ),
                }
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
