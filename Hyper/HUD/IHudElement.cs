using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.HUD;

internal interface IHudElement
{
    public bool Visible { get; set; }

    public void Render(Shader shader);
}
