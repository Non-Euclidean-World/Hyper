using OpenTK.Mathematics;

namespace Hyper.MarchingCubes
{
    internal struct Triangle
    {
        internal Vector3 A, B, C;
        internal Vector3 Na, Nb, Nc; // normal vectors

        public Triangle(Vector3 a, Vector3 b, Vector3 c, Vector3 na, Vector3 nb, Vector3 nc)
        {
            A = a;
            B = b;
            C = c;

            Na = na;
            Nb = nb;
            Nc = nc;
        }
    }
}
