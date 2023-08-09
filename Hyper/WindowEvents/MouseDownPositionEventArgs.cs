using OpenTK.Windowing.Common;

namespace Hyper.WindowEvents;

public struct MouseDownPositionEventArgs
{
    public float X;
    
    public float Y;
    
    public MouseButtonEventArgs MouseDownEventArgs;
    
    public MouseDownPositionEventArgs(float x, float y, MouseButtonEventArgs mouseDownEventArgs)
    {
        X = x;
        Y = y;
        MouseDownEventArgs = mouseDownEventArgs;
    }
}