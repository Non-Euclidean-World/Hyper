using Hyper.Shaders.LightSourceShader;
using Hyper.Shaders.Shadow;
using OpenTK.Graphics.OpenGL4;

namespace Hyper.Controllers;

// TODO There should be no separate light source controller. Those should be parts of other bodies.
internal class LightSourcesController : IController
{
    private readonly Scene _scene;

    private readonly AbstractLightSourceShader _shader;
    
    private readonly AbstractShadowShader _shadowShader;

    public LightSourcesController(Scene scene, AbstractLightSourceShader shader, AbstractShadowShader shadowShader)
    {
        _scene = scene;
        _shader = shader;
        _shadowShader = shadowShader;
    }

    public void Render()
    {
        _shader.SetUp(_scene.Camera, _scene.Player.CurrentSphereId);

        foreach (var light in _scene.LightSources)
        {
            light.Render(_shader, _shader.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition);
        }
    }
    
    public void RenderShadows(int width, int height)
    {
        // TODO make it work for non-Euclidean spaces
        foreach (var lightSource in _scene.LightSources)
        {
            lightSource.CubeMap.SetUp();
            _shadowShader.SetUp(lightSource);
            
            foreach (var chunk in _scene.Chunks)
            {
                chunk.Render(_shadowShader, _shadowShader.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition);
            }
        }
        
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        GL.Viewport(0, 0, width, height);
    }

    public void Dispose()
    {
        _shader.Dispose();
    }
}