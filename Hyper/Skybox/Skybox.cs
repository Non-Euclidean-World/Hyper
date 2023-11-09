using Common;
using Hyper.Shaders.SkyboxShader;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.Skybox;
internal class Skybox
{
    /// <summary>
    /// Cubemap texture for the skybox
    /// </summary>
    public Texture Texture { get; private init; }

    /// <summary>
    /// Rotation of the skybox in radians
    /// </summary>
    public float RotationX { get; set; }

    private readonly string[] _dayFaces;

    private readonly string[] _nightFaces;

    private readonly string _resourceDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Skybox", "Resources");

    private readonly SkyboxResource _skyboxResource;

    public Skybox(float scale)
    {
        _dayFaces = new string[]
        {
            "Day/px.png",
            "Day/nx.png",
            "Day/ny.png",
            "Day/py.png",
            "Day/pz.png",
            "Day/nz.png",
        };

        _nightFaces = new string[]
        {
            "Night/px.png",
            "Night/nx.png",
            "Night/ny.png",
            "Night/py.png",
            "Night/pz.png",
            "Night/nz.png",
        };

        _skyboxResource = new(scale);

        string[] paths = new string[6];
        for (int i = 0; i < _dayFaces.Length; i++)
            paths[i] = Path.Combine(_resourceDir, _nightFaces[i]);

        Texture = Texture.LoadCubemap(paths);
    }

    public void Render(AbstractSkyboxShader shader)
    {
        var rotation = Matrix4.CreateFromAxisAngle(Vector3.UnitX, RotationX);
        shader.SetMatrix4("model", rotation);

        GL.DepthFunc(DepthFunction.Lequal);
        GL.BindVertexArray(_skyboxResource.Vao);
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.TextureCubeMap, Texture.Name);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        GL.BindVertexArray(0);
        GL.DepthFunc(DepthFunction.Less);
    }
}
