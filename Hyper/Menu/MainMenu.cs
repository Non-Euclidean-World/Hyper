﻿using Common;
using Hud.Shaders;
using Hud.Widgets;
using Hud.Widgets.Colors;
using Hud.Widgets.MultipleChildren;
using Hud.Widgets.NoChildren;
using Hud.Widgets.SingleChild;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.Menu;

public class MainMenu
{
    private enum SaveGridMode
    {
        Load,
        Delete
    }
    
    public event Action Resume = null!;
    public event Action<string> Load = null!;
    public event Action<string> Delete = null!;
    public event Action Quit = null!;
    
    private readonly IWindowHelper _windowHelper;

    private readonly HudShader _shader = HudShader.Create();

    private Widget _activeWidget;
    
    private readonly AppBar _appBar = new ();
    
    private readonly SaveGrid _saveGrid;
    
    private readonly Widget _saveGridScreen;
    
    private SaveGridMode _saveGridMode = SaveGridMode.Load;

    public MainMenu(IWindowHelper windowHelper)
    {
        _windowHelper = windowHelper;
        _activeWidget = _appBar;
        SetUpAppBar();
        (_saveGrid, _saveGridScreen) = GetSaveGrid();
    }
    
    public void Reload()
    {
        _saveGrid.Reload();
    }

    private Widget GetWidgetWrapped(Widget widget)
    {
        return new Row(
            alignment: Alignment.Greedy,
            children: new Widget[]
            {
                _appBar,
                widget
            }
        );
    }

    private void SetUpAppBar()
    {
        _appBar.Resume += () => 
        {
            _activeWidget = _appBar;
            Resume?.Invoke();
        };
        _appBar.Load += () =>
        {
            _saveGrid!.Title = "Load Game";
            _saveGridMode = SaveGridMode.Load;
            _activeWidget = _saveGridScreen!;
        };
        _appBar.Delete += () =>
        {
            _saveGrid!.Title = "Delete Game";
            _saveGridMode = SaveGridMode.Delete;
            _activeWidget = _saveGridScreen!;
        };
        _appBar.Quit += () => Quit?.Invoke();
    }

    private (SaveGrid, Widget) GetSaveGrid()
    {
        var saveGrid = new SaveGrid();
        saveGrid.OnSelected += (saveName) =>
        {
            switch (_saveGridMode)
            {
                case SaveGridMode.Load:
                    Load?.Invoke(saveName);
                    break;
                case SaveGridMode.Delete:
                    Delete?.Invoke(saveName);
                    break;
            }
        };
        var saveGridScreen = GetWidgetWrapped(saveGrid);

        return (saveGrid, saveGridScreen);
    }
    
    public void Render()
    {
        GL.Disable(EnableCap.DepthTest);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        var aspectRatio = _windowHelper.GetAspectRatio();
        _shader.SetUp(aspectRatio);
        _shader.UseTexture(false);
        _shader.SetColor(Vector4.One);
        _activeWidget.Render(new Context(_shader, new Vector2(-aspectRatio / 2, 0.5f), new Vector2(aspectRatio, 1)));
        GL.Enable(EnableCap.DepthTest);
    }
    
    public void Click()
    {
        _activeWidget.Click(_windowHelper.GetMousePosition());
    }
    
    public void KeyDown(Keys key)
    {
        _activeWidget.KeyboardInput(key);
    }
}