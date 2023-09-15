using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using Hyper;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Wpf;
using MouseButtonEventArgs = System.Windows.Input.MouseButtonEventArgs;
using Window = System.Windows.Window;

namespace WpfMenu;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly Game _game;
    
    public MainWindow()
    {
        InitializeComponent();

        var settings = new GLWpfControlSettings();
        // You can start and rely on the Settings property that may be set in XAML or elsewhere in the codebase.
        OpenTkControl.Start(settings);

        _game = new Game((int)Width, (int)Height);
        _game.OnLoad();
    }
    
    private void OpenTkControl_OnRender(TimeSpan delta)
    {
        // TODO if game is saved delta still increases. fix it.
        _game.OnUpdateFrame(new FrameEventArgs(delta.Milliseconds / 1000.0));
        _game.OnRenderFrame(new FrameEventArgs(delta.Milliseconds / 1000.0));
        GL.Finish();
    }

    private void OpenTkControl_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            OpenTkControl.Visibility = Visibility.Collapsed;
            StartGameButton.Visibility = Visibility.Visible;
            QuitButton.Visibility = Visibility.Visible;
            Cursor = Cursors.Arrow;
            return;
        }
        
        if (GameHelper.KeysMap.TryGetValue(e.Key, out var value)) _game.OnKeyDown(value);
    }

    private void OpenTkControl_OnKeyUp(object sender, KeyEventArgs e)
    {
        if (GameHelper.KeysMap.TryGetValue(e.Key, out var value)) _game.OnKeyUp(value);
    }

    private void OpenTkControl_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (GameHelper.MouseButtonsMap.TryGetValue(e.ChangedButton, out var value)) _game.OnMouseDown(value);
    }

    private void OpenTkControl_OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (GameHelper.MouseButtonsMap.TryGetValue(e.ChangedButton, out var value)) _game.OnMouseUp(value);
    }

    private void StartGameButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (!_game.Loaded) _game.OnLoad();
        OpenTkControl.Visibility = Visibility.Visible;
        StartGameButton.Visibility = Visibility.Collapsed;
        QuitButton.Visibility = Visibility.Collapsed;
        Cursor = Cursors.None;
    }

    private void QuitButton_OnClick(object sender, RoutedEventArgs e)
    {
        _game.Close();
        Close();
    }
    
    private void CenterMouse()
    {
        int screenX = (int)(Left + Width / 2);
        int screenY = (int)(Top + Height / 2);
        GameHelper.SetCursorPos(screenX, screenY);
    }

    private void OpenTkControl_OnMouseMove(object sender, MouseEventArgs e)
    {
        GameHelper.GetCursorPos(out var newMousePos);
        CenterMouse();
        GameHelper.GetCursorPos(out var position);
        Point delta = new Point(newMousePos.X - position.X, newMousePos.Y - position.Y);

        // TODO: Update your in-game camera here
        _game.OnMouseMove(new MouseMoveEventArgs(
            new Vector2((float)newMousePos.X, (float)newMousePos.Y), 
            new Vector2((float)delta.X, (float)delta.Y)));
    }
}
