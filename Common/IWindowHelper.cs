using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Common;

public interface IWindowHelper
{
    CursorState CursorState { get; set; }

    Vector2 GetMousePosition();

    float GetAspectRatio();
}