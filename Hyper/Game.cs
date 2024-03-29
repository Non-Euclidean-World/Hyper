﻿using Chunks;
using Chunks.MarchingCubes;
using Common;
using Common.UserInput;
using Hyper.Controllers;
using Hyper.Controllers.Factories;
using Hyper.PlayerData;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper;

public class Game
{
    private readonly Scene _scene;

    private readonly IController[] _controllers;

    private readonly Context _context = new();

    private Vector2i _size;

    public readonly Settings Settings;

    private readonly float _globalScale = 0.05f;

    public Game(int width, int height, IWindowHelper windowHelper, string saveName, SelectedGeometryType selectedGeometryType) // TODO this is definitely getting out of hand
    {
        _size = new Vector2i(width, height);

        GL.ClearColor(0f, 0f, 0f, 1.0f);
        GL.Enable(EnableCap.DepthTest);

        if (!Settings.SaveExists(saveName))
        {
            Random rand = new Random();
            Settings = new Settings(rand.Next(), saveName, (float)width / height, selectedGeometryType);
        }
        else
        {
            Settings = Settings.Load(saveName); // TODO it's confusing as hell but geometryType variable is INVALID from this point onward
            Settings.AspectRatio = (float)width / height;
        }
#if DEBUG
        Console.WriteLine($"Seed: {Settings.Seed}");
#endif
        Settings.Save();

        float curve = Settings.SelectedGeometryType switch
        {
            SelectedGeometryType.Euclidean => 0f,
            SelectedGeometryType.Hyperbolic => -1f,
            SelectedGeometryType.Spherical => 1f,
            _ => throw new NotImplementedException(),
        };

        if (Settings.SelectedGeometryType == SelectedGeometryType.Spherical)
            Chunk.Size = 32;

        var scalarFieldGenerator = new ScalarFieldGenerator(Settings.Seed);
        var camera = new Camera(aspectRatio: _size.X / (float)_size.Y, curve, near: 0.01f, far: 200f, _globalScale, _context)
        {
            ReferencePointPosition = (5f + scalarFieldGenerator.AvgElevation) * Vector3.UnitY
        };
        _scene = new Scene(camera, _globalScale, _context);
        IControllerFactory controllerFactory = Settings.SelectedGeometryType switch
        {
            SelectedGeometryType.Spherical => new SphericalControllerFactory(_scene, _context, windowHelper, scalarFieldGenerator, _globalScale),
            SelectedGeometryType.Hyperbolic or SelectedGeometryType.Euclidean => new StandardControllerFactory(_scene, _context, windowHelper, scalarFieldGenerator),
            _ => throw new NotImplementedException(),
        };

        _controllers = controllerFactory.CreateControllers(Settings);
        _scene.SimulationManager.Start();
    }

    public void SaveAndClose()
    {
        foreach (var controller in _controllers)
        {
            controller.Dispose();
        }
        _scene.Dispose(); // Scene dispose needs to be after controller dispose.
        Settings.Save();
    }

    public void RenderFrame(FrameEventArgs e)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        foreach (var controller in _controllers)
        {
            controller.Render();
        }
    }

    public void UpdateFrame(FrameEventArgs e)
    {
        if (e.Time == 0) return;
        foreach (var callback in _context.FrameUpdateCallbacks)
        {
            callback(e);
        }
        _context.ExecuteAllHeldCallbacks(InputType.Key, e);
        _context.ExecuteAllHeldCallbacks(InputType.MouseButton, e);
    }

    public void KeyDown(Keys key)
    {
        if (!_context.KeyDownCallbacks.ContainsKey(key))
            return;

        foreach (var callback in _context.KeyDownCallbacks[key])
        {
            callback();
        }
    }

    public void KeyUp(Keys key)
    {
        if (!_context.KeyUpCallbacks.ContainsKey(key))
            return;

        foreach (var callback in _context.KeyUpCallbacks[key])
        {
            callback();
        }
    }

    public void MouseMove(MouseMoveEventArgs e)
    {
        foreach (var callback in _context.MouseMoveCallbacks)
        {
            callback(e);
        }
    }

    public void MouseDown(MouseButton button)
    {
        if (!_context.ButtonDownCallbacks.ContainsKey(button))
            return;

        foreach (var callback in _context.ButtonDownCallbacks[button])
        {
            callback();
        }
    }

    public void MouseUp(MouseButton button)
    {
        if (!_context.ButtonUpCallbacks.ContainsKey(button))
            return;

        foreach (var callback in _context.ButtonUpCallbacks[button])
        {
            callback();
        }
    }

    public void MouseWheel(MouseWheelEventArgs e)
    {
        HyperCameraPosition.Multiplier += e.OffsetY;
        HyperCameraPosition.Multiplier = Math.Min(HyperCameraPosition.Multiplier, 20f);
        HyperCameraPosition.Multiplier = Math.Max(HyperCameraPosition.Multiplier, 0f);
    }

    public void Resize(ResizeEventArgs e)
    {
        _scene.Camera.AspectRatio = e.Width / (float)e.Height;
        Settings.AspectRatio = e.Width / (float)e.Height;
        _size = e.Size;
    }

    public void StopClocks()
    {
        _scene.SimulationManager.Stop();
        foreach (var controller in _controllers)
        {
            if (controller is SkyboxController skyboxController)
            {
                skyboxController.Stop();
            }
        }
    }

    public void ResumeClocks()
    {
        _scene.SimulationManager.Start();
        foreach (var controller in _controllers)
        {
            if (controller is SkyboxController skyboxController)
            {
                skyboxController.Start();
            }
        }
    }
}