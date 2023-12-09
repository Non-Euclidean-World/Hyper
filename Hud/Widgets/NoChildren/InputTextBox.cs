using Hud.Widgets.Colors;
using Hud.Widgets.SingleChild;
using OpenTK.Mathematics;

namespace Hud.Widgets.NoChildren;
/// <summary>
/// A box in which you can enter text.
/// </summary>
public class InputTextBox : SingleChildWidget
{
    private readonly InputText _inputText;

    /// <summary>
    /// The content of the text box.
    /// </summary>
    public string Content => _inputText.Content;

    /// <summary>
    /// Creates an instance of InputTextBox class.
    /// </summary>
    /// <param name="text">Text in the text box.</param>
    /// <param name="size">Size od the text box.</param>
    /// <param name="placeholder">Whether the initial text should disappear when the user starts typing.</param>
    /// <param name="characterLimit">Maximum number of characters that can be in the text box.</param>
    /// <param name="color">Color of the background of the text box.</param>
    public InputTextBox(string text, Vector2 size, bool placeholder = false, int characterLimit = -1, Color color = Color.Primary)
    {
        _inputText = new InputText(
                            size: size.Y * 1 / 3,
                            text: text,
                            placeholder: placeholder,
                            characterLimit: characterLimit
                        );

        Child = new SizeBox(
            size: size,
            child: new ClickDetector(
                action: _inputText.Activate,
                outsideAction: _inputText.Deactivate,
                child: new Background(
                    color: color,
                    child: new Center(
                        child: _inputText
                    )
                )
            )
        );
    }
}
