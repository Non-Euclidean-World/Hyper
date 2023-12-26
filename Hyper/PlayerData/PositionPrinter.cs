using Common;
using Hud;
using Hyper.PlayerData.Utils;
using OpenTK.Mathematics;

namespace Hyper.PlayerData;

internal class PositionPrinter : IHudElement
{
    private readonly Camera _camera;

    private readonly IWindowHelper _windowHelper;

    private const float Size = 0.03f;

    public bool Visible { get; set; }

    private readonly float _globalScale;

    public PositionPrinter(Camera camera, float globalScale, IWindowHelper windowHelper)
    {
        _camera = camera;
        _windowHelper = windowHelper;
        Visible = true;
        _globalScale = globalScale;
    }

    public void Render(Shader shader)
    {
        shader.SetVector4("color", Vector4.One);
        Printer.RenderStringTopLeft(shader, GetPositionString(), Size, -_windowHelper.GetAspectRatio() / 2, 0.5f);
    }

    private string GetPositionString()
    {
        var position = _camera.ReferencePointPosition * _globalScale;
        if (MathF.Abs(_camera.Curve) < Constants.Eps)
        {
            return GetCoordinateString('x', position.X) +
                   GetCoordinateString('y', position.Y) +
                   GetCoordinateString('z', position.Z);
        }

        float d = position.LengthFast;
        if (_camera.Curve > 0 || d <= 3)
        {
            var positionNonEuc = GeomPorting.EucToCurved(position, _camera.Curve, _camera.Sphere, _camera.SphereCenter);
            return GetCoordinateString('x', positionNonEuc.X) +
                   GetCoordinateString('y', positionNonEuc.Y) +
                   GetCoordinateString('z', positionNonEuc.Z) +
                   GetCoordinateString('w', positionNonEuc.W);
        }

        Vector3 mul = position / d * 0.5f;
        return GetCoordinateString('x', $"{mul.X:0.0}*exp({d:0.0})") +
               GetCoordinateString('y', $"{mul.Y:0.0}*exp({d:0.0})") +
               GetCoordinateString('z', $"{mul.Z:0.0}*exp({d:0.0})") +
               GetCoordinateString('w', $"0.5*exp({d:0.0})");

    }

    private static string GetCoordinateString(char coordinate, float value)
    {
        return $"{coordinate} = {value:0.0}\n";
    }

    private static string GetCoordinateString(char coordinate, string value)
    {
        return $"{coordinate} = {value}\n";
    }

    public void Dispose()
    {
        // Nothing to dispose
    }
}