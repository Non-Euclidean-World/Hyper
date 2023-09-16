using System.Runtime.InteropServices;
using System.Windows.Input;
using Common;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace WpfMenu;

public class WpfWindowHelper : IWindowHelper
{
    private readonly MainWindow _window;

    public bool IsCursorGrabbed { get; private set; } = true;
    
    public WpfWindowHelper(MainWindow window)
    {
        _window = window;
    }

    public CursorState CursorState
    {
        get
        {
            if (IsCursorGrabbed) return CursorState.Grabbed;
            if (_window.Cursor == Cursors.None) return CursorState.Hidden;
            return CursorState.Normal;
        }
        set
        {
            if (value == CursorState.Grabbed)
            {
                IsCursorGrabbed = true;
                _window.Cursor = Cursors.None;
            }
            else if (value == CursorState.Hidden)
            {
                IsCursorGrabbed = false;
                _window.Cursor = Cursors.None;
            }
            else
            {
                IsCursorGrabbed = false;
                _window.Cursor = Cursors.Arrow;
            }
        }
    }

    public Vector2 GetMousePosition()
    {
        var mouse = Mouse.GetPosition(_window.OpenTkControl);
        var aspectRatio = (float)(_window.OpenTkControl.ActualWidth / _window.OpenTkControl.ActualHeight);

        return new Vector2(
            (float)(mouse.X / _window.OpenTkControl.ActualWidth) * aspectRatio - aspectRatio * 0.5f,
            0.5f - (float)(mouse.Y / _window.OpenTkControl.ActualHeight));
    }

    public float GetAspectRatio() => (float)(_window.OpenTkControl.ActualWidth / _window.OpenTkControl.ActualHeight);
    
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