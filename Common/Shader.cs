using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Common;

public class Shader
{
    private readonly int _handle;

    private readonly Dictionary<string, int> _uniformLocations;

    public Shader((string path, ShaderType shaderType)[] shaders)
    {
        _handle = GL.CreateProgram();

        var shadersToDelete = new List<int>();
        foreach (var shader in shaders)
        {
            var createdShader = CreateShader(shader.path, shader.shaderType);
            shadersToDelete.Add(createdShader);
        }

        LinkProgram(_handle);

        foreach (var shader in shadersToDelete)
        {
            GL.DetachShader(_handle, shader);
            GL.DeleteShader(shader);
        }

        GL.GetProgram(_handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

        _uniformLocations = new Dictionary<string, int>();

        for (var i = 0; i < numberOfUniforms; i++)
        {
            var key = GL.GetActiveUniform(_handle, i, out int _, out _);

            var location = GL.GetUniformLocation(_handle, key);
            _uniformLocations.Add(key, location);
        }
    }

    public void Use()
    {
        GL.UseProgram(_handle);
    }

    /// <summary>
    /// Sets a uniform int on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform</param>
    /// <param name="data">The data to set</param>
    public void SetInt(string name, int data)
    {
        GL.UseProgram(_handle);
        GL.Uniform1(_uniformLocations[name], data);
    }

    /// <summary>
    /// Sets a uniform float on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform</param>
    /// <param name="data">The data to set</param>
    public void SetFloat(string name, float data)
    {
        GL.UseProgram(_handle);
        GL.Uniform1(_uniformLocations[name], data);
    }

    /// <summary>
    /// Sets a uniform Matrix4 on this shader
    /// </summary>
    /// <param name="name">The name of the uniform</param>
    /// <param name="data">The data to set</param>
    /// <remarks>
    ///   <para>
    ///   The matrix is transposed before being sent to the shader.
    ///   </para>
    /// </remarks>
    public void SetMatrix4(string name, Matrix4 data)
    {
        GL.UseProgram(_handle);
        GL.UniformMatrix4(_uniformLocations[name], true, ref data);
    }

    /// <summary>
    /// Sets a uniform Vector3 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform</param>
    /// <param name="data">The data to set</param>
    public void SetVector3(string name, Vector3 data)
    {
        GL.UseProgram(_handle);
        GL.Uniform3(_uniformLocations[name], data);
    }

    /// <summary>
    /// Sets a uniform Vector4 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform</param>
    /// <param name="data">The data to set</param>
    public void SetVector4(string name, Vector4 data)
    {
        GL.UseProgram(_handle);
        GL.Uniform4(_uniformLocations[name], data);
    }

    /// <summary>
    /// Sets a uniform bool on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform</param>
    /// <param name="data">The data to set</param>
    public void SetBool(string name, bool data)
    {
        GL.UseProgram(_handle);
        GL.Uniform1(_uniformLocations[name], data ? 1 : 0);
    }

    /// <summary>
    /// Sets a uniform array of Vector3 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform</param>
    /// <param name="data">The data to set</param>
    public void SetVector3Array(string name, Vector3[] data)
    {
        float[] vectorData = new float[data.Length * 3];
        for (int i = 0; i < data.Length; i++)
        {
            vectorData[i * 3 + 0] = data[i].X;
            vectorData[i * 3 + 1] = data[i].Y;
            vectorData[i * 3 + 2] = data[i].Z;
        }
        GL.UseProgram(_handle);
        GL.Uniform3(_uniformLocations[name + "[0]"], data.Length, vectorData);
    }

    /// <summary>
    /// Sets a uniform array of Vector4 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform</param>
    /// <param name="data">The data to set</param>
    public void SetVector4Array(string name, Vector4[] data)
    {
        float[] vectorData = new float[data.Length * 4];
        for (int i = 0; i < data.Length; i++)
        {
            vectorData[i * 4 + 0] = data[i].X;
            vectorData[i * 4 + 1] = data[i].Y;
            vectorData[i * 4 + 2] = data[i].Z;
            vectorData[i * 4 + 3] = data[i].W;
        }
        GL.UseProgram(_handle);
        GL.Uniform4(_uniformLocations[name + "[0]"], data.Length, vectorData);
    }

    /// <summary>
    /// Sets a uniform array of Matrix4 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform</param>
    /// <param name="data">The data to set</param>
    public void SetMatrix4Array(string name, Matrix4[] data)
    {
        float[] matrixData = new float[data.Length * 16];
        for (int i = 0; i < data.Length; i++)
        {
            var matrix = data[i];
            matrixData[i * 16 + 0] = matrix.M11;
            matrixData[i * 16 + 1] = matrix.M12;
            matrixData[i * 16 + 2] = matrix.M13;
            matrixData[i * 16 + 3] = matrix.M14;
            matrixData[i * 16 + 4] = matrix.M21;
            matrixData[i * 16 + 5] = matrix.M22;
            matrixData[i * 16 + 6] = matrix.M23;
            matrixData[i * 16 + 7] = matrix.M24;
            matrixData[i * 16 + 8] = matrix.M31;
            matrixData[i * 16 + 9] = matrix.M32;
            matrixData[i * 16 + 10] = matrix.M33;
            matrixData[i * 16 + 11] = matrix.M34;
            matrixData[i * 16 + 12] = matrix.M41;
            matrixData[i * 16 + 13] = matrix.M42;
            matrixData[i * 16 + 14] = matrix.M43;
            matrixData[i * 16 + 15] = matrix.M44;
        }

        GL.UseProgram(_handle);
        GL.UniformMatrix4(_uniformLocations[name + "[0]"], data.Length, true, matrixData);
    }

    private int CreateShader(string shaderPath, ShaderType shaderType)
    {
        var shaderSource = File.ReadAllText(shaderPath);
        var vertexShader = GL.CreateShader(shaderType);
        GL.ShaderSource(vertexShader, shaderSource);
        CompileShader(vertexShader);
        GL.AttachShader(_handle, vertexShader);

        return vertexShader;
    }

    private static void CompileShader(int shader)
    {
        GL.CompileShader(shader);
        GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
        if (code != (int)All.True)
        {
            var infoLog = GL.GetShaderInfoLog(shader);
            throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
        }
    }

    private static void LinkProgram(int program)
    {
        GL.LinkProgram(program);

        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
        if (code != (int)All.True)
        {
            var infoLog = GL.GetProgramInfoLog(program);
            throw new Exception($"Error occurred whilst linking Program({program}).\n\n{infoLog}");
        }
    }
}
