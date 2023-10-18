using OpenTK.Graphics.OpenGL4;

namespace Common;

public class CubeMap
{
    private readonly int _handle;
    
    private readonly int _depthMapFBO;
    
    public readonly int Width;
    
    public readonly int Height;
        
    public CubeMap(int width = 1024, int height = 1024)
    {
        Width = width;
        Height = height;
        
        _handle = GL.GenTexture();
        GL.BindTexture(TextureTarget.TextureCubeMap, _handle);
        for (int i = 0; i < 6; ++i)
        {
            GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.DepthComponent,
                width, height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
        }
        
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

        _depthMapFBO = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _depthMapFBO);
        GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, _handle, 0);
        GL.DrawBuffer(DrawBufferMode.None);
        GL.ReadBuffer(ReadBufferMode.None);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public void SetUp()
    {
        GL.Viewport(0, 0, Width, Height);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _depthMapFBO);
        GL.Clear(ClearBufferMask.DepthBufferBit);
    }

    public void Use(TextureUnit textureUnit)
    {
        GL.ActiveTexture(textureUnit);
        GL.BindTexture(TextureTarget.TextureCubeMap, _handle);
    }
}