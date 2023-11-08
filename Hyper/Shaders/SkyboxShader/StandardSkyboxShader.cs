namespace Hyper.Shaders.SkyboxShader;
internal class StandardSkyboxShader : AbstractSkyboxShader
{
    public static StandardSkyboxShader Create(float globalScale) => new StandardSkyboxShader(globalScale);

    private StandardSkyboxShader(float scale) : base(scale)
    { }

}
