using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Hyper.MarchingCubes
{
    internal class Renderer
    {
        private float _isolevel;
        private float[,,] _voxels;

        public Renderer(float[,,] voxels, float isolevel = 0.5f)
        {
            _voxels = voxels;
            _isolevel = isolevel;
        }

        public Triangle[] GetMesh()
        {
            var triangles = new List<Triangle>();
            for (int x = 0; x < _voxels.GetLength(0) - 1; x++)
            {
                for (int y = 0; y < _voxels.GetLength(1) - 1; y++)
                {
                    for (int z = 0; z < _voxels.GetLength(2) - 1; z++)
                    {
                        GetTriangles(triangles, x, y, z);
                    }
                }
            }

            return triangles.ToArray();
        }

        private void GetTriangles(List<Triangle> triangles, int x, int y, int z)
        {
            var cubeValues = GetCubeValues(x, y, z);
            var edges = GetEdges(cubeValues);
            var position = new Vector3(x, y, z);

            for (int i = 0; edges[i] != -1; i += 3)
            {
                int e00 = MarchingCubesTables.EdgeConnections[edges[i]][0];
                int e01 = MarchingCubesTables.EdgeConnections[edges[i]][1];

                int e10 = MarchingCubesTables.EdgeConnections[edges[i + 1]][0];
                int e11 = MarchingCubesTables.EdgeConnections[edges[i + 1]][1];

                int e20 = MarchingCubesTables.EdgeConnections[edges[i + 2]][0];
                int e21 = MarchingCubesTables.EdgeConnections[edges[i + 2]][1];

                var a = Interpolate(MarchingCubesTables.CubeCorners[e00], cubeValues[e00], MarchingCubesTables.CubeCorners[e01], cubeValues[e01]) + position;
                var b = Interpolate(MarchingCubesTables.CubeCorners[e10], cubeValues[e10], MarchingCubesTables.CubeCorners[e11], cubeValues[e11]) + position;
                var c = Interpolate(MarchingCubesTables.CubeCorners[e20], cubeValues[e20], MarchingCubesTables.CubeCorners[e21], cubeValues[e21]) + position;
                triangles.Add(new Triangle(a, b, c));
            }
        }

        private int[] GetEdges(float[] cubeValues)
        {
            int cubeIndex = 0;
            if (cubeValues[0] < _isolevel) cubeIndex |= 1;
            if (cubeValues[1] < _isolevel) cubeIndex |= 2;
            if (cubeValues[2] < _isolevel) cubeIndex |= 4;
            if (cubeValues[3] < _isolevel) cubeIndex |= 8;
            if (cubeValues[4] < _isolevel) cubeIndex |= 16;
            if (cubeValues[5] < _isolevel) cubeIndex |= 32;
            if (cubeValues[6] < _isolevel) cubeIndex |= 64;
            if (cubeValues[7] < _isolevel) cubeIndex |= 128;

            return MarchingCubesTables.TriTable[cubeIndex];
        }
        private Vector3 Interpolate(Vector3 edgeVertex1, float valueAtVertex1, Vector3 edgeVertex2, float valueAtVertex2)
        {
            return edgeVertex1 + (_isolevel - valueAtVertex1) * (edgeVertex2 - edgeVertex1) / (valueAtVertex2 - valueAtVertex1);
        }

        private float[] GetCubeValues(int x, int y, int z)
        {
            float[] cubeValues = new float[8];
            for (int i = 0; i < 8; i++)
            {
                var offset = MarchingCubesTables.CubeCorners[i];
                cubeValues[i] = _voxels[x + offset.X, y + offset.Y, z + offset.Z];
            }

            return cubeValues;
        }
    }
}
