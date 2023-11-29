using OpenTK.Graphics.OpenGL4;
using SkiaSharp;
using StbImageSharp;

namespace Common;

public class Texture
{
    private readonly int _handle;

    public int Name { get => _handle; }

    /// <summary>
    /// Loads a texture from a file.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Texture LoadFromFile(string path)
    {
        int handle = GL.GenTexture();

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, handle);

        StbImage.stbi_set_flip_vertically_on_load(1);

        using (Stream stream = File.OpenRead(path))
        {
            ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        }

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        return new Texture(handle);
    }

    /// <summary>
    /// Loads a texture from a bitmap.
    /// </summary>
    /// <param name="bitmap"></param>
    /// <returns></returns>
    public static Texture LoadFromBitmap(SKBitmap bitmap)
    {
        int handle = GL.GenTexture();

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, handle);

        StbImage.stbi_set_flip_vertically_on_load(1);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bitmap.GetPixels());

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        return new Texture(handle);
    }

    /// <summary>
    /// Loads a cubemap from files.
    /// </summary>
    /// <param name="paths">Paths to files with faces of the cubemap</param>
    /// <returns></returns>
    public static Texture LoadCubemap(string[] paths)
    {
        int handle = GL.GenTexture();
        GL.BindTexture(TextureTarget.TextureCubeMap, handle);

        for (int i = 0; i < paths.Length; i++)
        {
            var face = paths[i];
            using Stream stream = File.OpenRead(face);
            ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlue);

            GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0,
                PixelInternalFormat.Rgb, image.Width, image.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, image.Data);
        }

        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

        return new Texture(handle);
    }

    private Texture(int glHandle)
    {
        _handle = glHandle;
    }

    public void Use(TextureUnit unit)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, _handle);
    }

    public void Dispose()
    {
        GL.DeleteTexture(_handle);
    }
}
