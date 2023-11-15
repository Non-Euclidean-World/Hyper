using Hud.Widgets.NoChildren;
using OpenTK.Mathematics;

namespace Hud.Widgets.MultipleChildren;

/// <summary>
/// Creates a grid of widgets.
/// </summary>
public class Grid : MultipleChildrenWidget
{
    private readonly Column _child;

    /// <summary>
    /// Creates an instance of Grid class.
    /// </summary>
    /// <param name="children">Array of child widgets.</param>
    /// <param name="numberOfChildrenInARow">Number of children in each row.</param>
    /// <param name="alignment">The way to aligns the children.</param>
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

    /// <summary>
    /// Creates an instance of Grid class based on a method that creates widgets from T.
    /// </summary>
    /// <param name="children">A IEnumerable of children.</param>
    /// <param name="widgetMaker">Method that converts children into Widgets.</param>
    /// <param name="numberOfChildrenInARow">Number of children in each row.</param>
    /// <param name="alignment">The way to aligns the children.</param>
    /// <typeparam name="T">Any type.</typeparam>
    /// <returns>Instance of Grid class.</returns>
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