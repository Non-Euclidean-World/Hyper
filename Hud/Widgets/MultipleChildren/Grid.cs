using Hud.Widgets.NoChildren;
using OpenTK.Mathematics;

namespace Hud.Widgets.MultipleChildren;

public class Grid : MultipleChildrenWidget
{
    private readonly Column _child;

    public Grid(Widget[] children, int numberOfChildrenInARow, Alignment alignment = Alignment.Equal) : base(children)
    {
        var colChildren = new List<Widget>();
        for (int i = 0; i < children.Length; i += numberOfChildrenInARow)
        {
            var rowChildren = new List<Widget>();
            for (int j = 0; j < numberOfChildrenInARow; j++)
            {
                if (i + j < children.Length)
                    rowChildren.Add(children[i + j]);
                else
                    rowChildren.Add(new Empty());
            }
            colChildren.Add(new Row(rowChildren.ToArray(), alignment));
        }

        _child = new Column(colChildren.ToArray(), alignment);
    }

    public static Grid Build<T>(IEnumerable<T> children, Func<T, Widget> widgetMaker, int numberOfChildrenInARow, Alignment alignment = Alignment.Equal)
    {
        var widgets = children.Select(widgetMaker).ToArray();
        return new Grid(widgets, numberOfChildrenInARow, alignment);
    }

    public override Vector2 GetSize()
    {
        return _child.GetSize();
    }

    public override void Render(Context context)
    {
        _child.Render(context);
    }
}