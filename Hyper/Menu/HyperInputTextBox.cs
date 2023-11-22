using Hud.Widgets.NoChildren;
using Hud.Widgets.SingleChild;
using OpenTK.Mathematics;

namespace Hyper.Menu;
/// <summary>
/// Input text box used in the game. It is the same as a <see cref="InputTextBox"/> but with some extra styling.
/// </summary>
internal class HyperInputTextBox : SingleChildWidget
{
    private readonly InputTextBox _textBox;

    public string Text => _textBox.Content;

    private readonly Background _background;

    public Vector4 Color { get => _background.Color; set => _background.Color = value; }

    public HyperInputTextBox(string placeholderText, Vector2 size)
    {
        _textBox = new InputTextBox(
            text: placeholderText,
            placeholder: true,
            characterLimit: (int)(size.X * 40),
            size: size - new Vector2(0.02f));

        _background = new Background(
            color: Hud.Widgets.Colors.Color.Secondary,
            child: new Padding(
                size: 0.01f,
                child: _textBox
            )
        );

        Child = new Padding(
            size: 0.01f,
            child: _background
        );
    }
}
