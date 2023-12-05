namespace Hyper.Shaders.ObjectShader;
internal class StandardObjectShader : AbstractObjectShader
{
    private StandardObjectShader() : base()
    {
    }

    public static StandardObjectShader Create()
    {
        return new StandardObjectShader();
    }
}
