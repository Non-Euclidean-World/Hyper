using OpenTK.Mathematics;

namespace Hyper.MarchingCubes
{
    internal struct Triangle
    {
        internal Vector3 A, B, C;

        public Triangle(Vector3 a, Vector3 b, Vector3 c)
        {
            A = a;
            B = b;
            C = c;
        }
    }
}
