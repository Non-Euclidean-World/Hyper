using System.Runtime.InteropServices;
using System.Windows.Input;
using Common;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Wpf;

namespace WpfMenu;

public class WpfWindowHelper : IWindowHelper
{
    private readonly GLWpfControl _glControl;

    public bool IsCursorGrabbed { get; private set; } = true;
    
    public WpfWindowHelper(GLWpfControl glControl)
    {
        _glControl = glControl;
    }

    public CursorState CursorState
    {
        get
        {
            if (IsCursorGrabbed) return CursorState.Grabbed;
            if (_glControl.Cursor == Cursors.None) return CursorState.Hidden;
            return CursorState.Normal;
        }
        set
        {
            if (value == CursorState.Grabbed)
            {
                IsCursorGrabbed = true;
                _glControl.Cursor = Cursors.None;
            }
            else if (value == CursorState.Hidden)
            {
                IsCursorGrabbed = false;
                _glControl.Cursor = Cursors.None;
            }
            else
            {
                IsCursorGrabbed = false;
                _glControl.Cursor = Cursors.Arrow;
            }
        }
    }

    public Vector2 GetMousePosition()
    {
        var mouse = Mouse.GetPosition(_glControl);
        var aspectRatio = (float)(_glControl.ActualWidth / _glControl.ActualHeight);

        return new Vector2(
            (float)(mouse.X / _glControl.ActualWidth) * aspectRatio - aspectRatio * 0.5f,
            0.5f - (float)(mouse.Y / _glControl.ActualHeight));
    }

    public float GetAspectRatio() => (float)(_glControl.ActualWidth / _glControl.ActualHeight);
    
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int X;
        public int Y;
    }
    
    [DllImport("User32.dll")]
    public static extern bool SetCursorPos(int x, int y);
    
    [DllImport("User32.dll")]
    public static extern bool GetCursorPos(out Point lpPoint);
}