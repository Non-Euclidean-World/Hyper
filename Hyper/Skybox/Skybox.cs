using Common;
using Hyper.Shaders.SkyboxShader;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.Skybox;

/// <summary>
/// Class representing a skybox
/// </summary>
internal class Skybox
{
    /// <summary>
    /// Rotation of the skybox in radians
    /// </summary>
    public float RotationX { get; set; }

    private readonly Texture _texture;

    private readonly string[] _nightFaces;

    private readonly string _resourceDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Skybox", "Resources");

    private readonly SkyboxResource _skyboxResource;

    public Skybox()
    {
        _nightFaces = new string[]
        {
            "Night/px.png",
            "Night/nx.png",
            "Night/ny.png",
            "Night/py.png",
            "Night/pz.png",
            "Night/nz.png",
        };

        _skyboxResource = new();

        string[] paths = new string[6];
        for (int i = 0; i < _nightFaces.Length; i++)
            paths[i] = Path.Combine(_resourceDir, _nightFaces[i]);

        _texture = Texture.LoadCubemap(paths);
    }

    /// <summary>
    /// Renders the skybox
    /// </summary>
    /// <param name="shader"></param>
    public void Render(AbstractSkyboxShader shader)
    {
        var rotation = Matrix4.CreateFromAxisAngle(Vector3.UnitX, RotationX);
        shader.SetMatrix4("model", rotation);

        GL.DepthFunc(DepthFunction.Lequal);
        GL.BindVertexArray(_skyboxResource.Vao);
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.TextureCubeMap, _texture.Name);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        GL.BindVertexArray(0);
        GL.DepthFunc(DepthFunction.Less);
    }
}
