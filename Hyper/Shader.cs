using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper
{
    internal class Shader
    {
        private readonly int _handle;

        private readonly Dictionary<string, int> _uniformLocations;

        internal Shader((string path, ShaderType shaderType)[] shaders)
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
                var key = GL.GetActiveUniform(_handle, i, out int size, out _);

                if (size > 1)
                {
                    string baseName = key.Split('[')[0];
                    for (int j = 0; j < size; j++)
                    {
                        var arrayKey = baseName + "[" + j + "]";
                        var location = GL.GetUniformLocation(_handle, arrayKey);
                        _uniformLocations.Add(arrayKey, location);
                    }
                }
                else
                {
                    var location = GL.GetUniformLocation(_handle, key);
                    _uniformLocations.Add(key, location);
                }
            }
        }

        internal void Use()
        {
            GL.UseProgram(_handle);
        }

        internal int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(_handle, attribName);
        }

        /// <summary>
        /// Set a uniform int on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        internal void SetInt(string name, int data)
        {
            GL.UseProgram(_handle);
            GL.Uniform1(_uniformLocations[name], data);
        }

        /// <summary>
        /// Set a uniform float on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        internal void SetFloat(string name, float data)
        {
            GL.UseProgram(_handle);
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
        internal void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(_handle);
            GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        }

        /// <summary>
        /// Set a uniform Vector3 on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        internal void SetVector3(string name, Vector3 data)
        {
            GL.UseProgram(_handle);
            GL.Uniform3(_uniformLocations[name], data);
        }

        internal void SetVector4(string name, Vector4 data)
        {
            GL.UseProgram(_handle);
            GL.Uniform4(_uniformLocations[name], data);
        }

        /// <summary>
        /// Set a uniform bool on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        internal void SetBool(string name, bool data)
        {
            GL.UseProgram(_handle);
            GL.Uniform1(_uniformLocations[name], data ? 1 : 0);
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
}
