using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper
{
    public class Shader
    {
        public readonly int Handle;

        private readonly Dictionary<string, int> _uniformLocations;

        public Shader((string path, ShaderType shaderType)[] shaders)
        {
            Handle = GL.CreateProgram();

            var shadersToDelete = new List<int>();
            foreach (var shader in shaders)
            {
                var createdShader = CreateShader(shader.path, shader.shaderType);
                shadersToDelete.Add(createdShader);
            }

            LinkProgram(Handle);

            foreach (var shader in shadersToDelete)
            {
                GL.DetachShader(Handle, shader);
                GL.DeleteShader(shader);
            }

            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            _uniformLocations = new Dictionary<string, int>();

            for (var i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(Handle, i, out int size, out ActiveUniformType type);

                if (size > 1)
                {
                    string baseName = key.Split('[')[0];
                    for (int j = 0; j < size; j++)
                    {
                        var arrayKey = baseName + "[" + j + "]";
                        var location = GL.GetUniformLocation(Handle, arrayKey);
                        _uniformLocations.Add(arrayKey, location);
                    }
                }
                else
                {
                    var location = GL.GetUniformLocation(Handle, key);
                    _uniformLocations.Add(key, location);
                }
            }
        }

        private int CreateShader(string shaderPath, ShaderType shaderType)
        {
            var shaderSource = File.ReadAllText(shaderPath);
            var vertexShader = GL.CreateShader(shaderType);
            GL.ShaderSource(vertexShader, shaderSource);
            CompileShader(vertexShader);
            GL.AttachShader(Handle, vertexShader);

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

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }

        /// <summary>
        /// Set a uniform int on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetInt(string name, int data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(_uniformLocations[name], data);
        }

        /// <summary>
        /// Set a uniform float on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetFloat(string name, float data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(_uniformLocations[name], data);
        }

        /// <summary>
        /// Set a uniform Matrix4 on this shader
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
            GL.UseProgram(Handle);
            GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        }

        /// <summary>
        /// Set a uniform Vector3 on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetVector3(string name, Vector3 data)
        {
            GL.UseProgram(Handle);
            GL.Uniform3(_uniformLocations[name], data);
        }

        public void SetVector4(string name, Vector4 data)
        {
            GL.UseProgram(Handle);
            GL.Uniform4(_uniformLocations[name], data);
        }
    }
}
