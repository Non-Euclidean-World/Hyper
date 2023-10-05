

using Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Player;

namespace Hyper.Shaders;
public class AbstractLightSourceShader : Shader
{
    public float GlobalScale { get; private init; }

    protected AbstractLightSourceShader((string path, ShaderType shaderType)[] shaders, float globalScale)
        : base(shaders)
    {
        GlobalScale = globalScale;
    }

    public void SetCurv(float curv) => SetFloat("curv", curv);

    public void SetView(Matrix4 view) => SetMatrix4("view", view);

    public void SetProjection(Matrix4 projection) => SetMatrix4("projection", projection);

    public virtual void SetUp(Camera camera, int sphere = 0)
    {
        Use();
        SetCurv(camera.Curve);
        SetView(camera.GetViewMatrix());
        SetProjection(camera.GetProjectionMatrix());
    }
}
