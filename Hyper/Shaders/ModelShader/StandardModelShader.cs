namespace Hyper.Shaders.ModelShader;
internal class StandardModelShader : AbstractModelShader
{
    private StandardModelShader()
        : base()
    { }

    public static StandardModelShader Create()
    {
        return new StandardModelShader();
    }
}
