using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace Hyper.Meshes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector3 Position;
        public Vector2 TexCoords;
    }
}
