using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hud.Widgets.Colors;
using Hud.Widgets.SingleChild;
using OpenTK.Mathematics;

namespace Hud.Widgets.NoChildren;
public class InputTextBox : SingleChildWidget
{
    private InputText _inputText;

    public string Content => _inputText.Content;

    public InputTextBox(string text, Vector2 size, bool placeholder = false, int characterlimit = -1, Color color = Color.Primary)
    {
        _inputText = new InputText(
                            size: size.Y * 1 / 3,
                            text: text,
                            placeholder: placeholder,
                            characterlimit: characterlimit
                        );

        Child = new SizeBox(
            size: size,
            child: new ClickDetector(
                action: _inputText.Activate,
                outsideAction: _inputText.Desactivate,
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
