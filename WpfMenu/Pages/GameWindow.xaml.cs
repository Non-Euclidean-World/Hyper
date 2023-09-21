using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hyper;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Wpf;

namespace WpfMenu.Pages;

public partial class GameWindow : UserControl
{
    private Game _game = null!;

    private readonly WpfWindowHelper _windowHelper;

    public GameWindow()
    {
        InitializeComponent();
        _windowHelper = new WpfWindowHelper(OpenTkControl);

        var settings = new GLWpfControlSettings();
        OpenTkControl.Start(settings);
    }

    public void Load(int width, int height, string name)
    {
        Visibility = Visibility.Visible;
        _game = new Game(width, height, _windowHelper);
        _game.Resize(new ResizeEventArgs(width, height));
        Cursor = Cursors.None;
    }

    private void OpenTkControl_OnRender(TimeSpan delta)
    {
        // TODO if game is saved delta still increases. fix it.
        _game.UpdateFrame(new FrameEventArgs(delta.Milliseconds / 1000.0));
        _game.RenderFrame(new FrameEventArgs(delta.Milliseconds / 1000.0));
        GL.Finish();
    }

    private void OpenTkControl_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _game.Resize(new ResizeEventArgs((int)ActualWidth, (int)ActualHeight));
    }

    private void OpenTkControl_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (Visibility != Visibility.Visible) return;

        if (e.Key == Key.Escape)
        {
            OpenTkControl.Visibility = Visibility.Collapsed;
            MenuPanel.Visibility = Visibility.Visible;
            Cursor = Cursors.Arrow;
            return;
        }

        if (GameHelper.KeysMap.TryGetValue(e.Key, out var value)) _game.KeyDown(value);
    }

    private void OpenTkControl_OnKeyUp(object sender, KeyEventArgs e)
    {
        if (Visibility != Visibility.Visible) return;

        if (GameHelper.KeysMap.TryGetValue(e.Key, out var value)) _game.KeyUp(value);
    }

    private void OpenTkControl_OnMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (GameHelper.MouseButtonsMap.TryGetValue(e.ChangedButton, out var value)) _game.MouseDown(value);
    }

    private void OpenTkControl_OnMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (GameHelper.MouseButtonsMap.TryGetValue(e.ChangedButton, out var value)) _game.MouseUp(value);
    }

    private void OpenTkControl_OnMouseMove(object sender, MouseEventArgs e)
    {
        WpfWindowHelper.GetCursorPos(out var newMousePos);
        if (_windowHelper.IsCursorGrabbed) CenterMouse();
        WpfWindowHelper.GetCursorPos(out var position);
        Point delta = new Point(newMousePos.X - position.X, newMousePos.Y - position.Y);

        _game.MouseMove(new MouseMoveEventArgs(
            new Vector2(newMousePos.X, newMousePos.Y),
            new Vector2((float)delta.X, (float)delta.Y)));
    }

    private void CenterMouse()
    {
        var point = PointToScreen(new Point(0, 0));
        int screenX = (int)(point.X + ActualWidth / 2);
        int screenY = (int)(point.Y + ActualHeight / 2);
        WpfWindowHelper.SetCursorPos(screenX, screenY);
    }

    private void ResumeButton_OnClick(object sender, RoutedEventArgs e)
    {
        CenterMouse();
        MenuPanel.Visibility = Visibility.Collapsed;
        OpenTkControl.Visibility = Visibility.Visible;
        Cursor = Cursors.None;
    }

    private void SaveAndQuitButton_OnClick(object sender, RoutedEventArgs e)
    {
        OpenTkControl.Visibility = Visibility.Visible;
        MenuPanel.Visibility = Visibility.Collapsed;
        Visibility = Visibility.Collapsed;
        _game.Close();
        Cursor = Cursors.Arrow;
    }
}