namespace Hyper.MarchingCubes
{
    internal class PerlinNoise
    {
        private readonly int[] permutation;

        internal PerlinNoise(int seed)
        {
            var random = new Random(seed);
            permutation = Enumerable.Range(0, 256).OrderBy(x => random.Next()).ToArray();
        }

        internal float GetNoise3D(float x, float y, float z)
        {
            float px = x * 0.1553f;
            float py = y * 0.1271f;
            float pz = z * 0.0916f;

            float noise = (GetNoise(px, py, pz) + 1) / 2.0f;

            return noise;
        }

        internal float GetNoise(float x, float y, float z)
        {
            // Calculate the unit cube that contains the point
            int cubeX = (int)Math.Floor(x) & 255;
            int cubeY = (int)Math.Floor(y) & 255;
            int cubeZ = (int)Math.Floor(z) & 255;

            // Relative location of point in the cube
            x -= (int)Math.Floor(x);
            y -= (int)Math.Floor(y);
            z -= (int)Math.Floor(z);

            // Compute fade curves for each of the cube's dimensions
            float fadeX = Fade(x);
            float fadeY = Fade(y);
            float fadeZ = Fade(z);

            // Hash coordinates of the cube's 8 corners
            int a = Permutate(cubeX) + cubeY;
            int aa = Permutate(a) + cubeZ;
            int ab = Permutate(a + 1) + cubeZ;
            int b = Permutate(cubeX + 1) + cubeY;
            int ba = Permutate(b) + cubeZ;
            int bb = Permutate(b + 1) + cubeZ;

            // Compute the gradient dot products and blend them together
            float u = Lerp(Gradient(Permutate(aa), x, y, z),
                           Gradient(Permutate(ba), x - 1, y, z),
                           fadeX);
            float v = Lerp(Gradient(Permutate(ab), x, y - 1, z),
                           Gradient(Permutate(bb), x - 1, y - 1, z),
                           fadeX);
            float w = Lerp(u, v, fadeY);

            u = Lerp(Gradient(Permutate(aa + 1), x, y, z - 1),
                     Gradient(Permutate(ba + 1), x - 1, y, z - 1),
                     fadeX);
            v = Lerp(Gradient(Permutate(ab + 1), x, y - 1, z - 1),
                     Gradient(Permutate(bb + 1), x - 1, y - 1, z - 1),
                     fadeX);
            float t = Lerp(u, v, fadeY);

            // Final blend
            return Lerp(w, t, fadeZ);
        }

        private int Permutate(int x)
        {
            return permutation[x & 255];
        }

        private static float Fade(float t)
        {
            // Fade function as defined by Ken Perlin
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        private static float Lerp(float a, float b, float t)
        {
            return a + t * (b - a);
        }

        private static float Gradient(int hash, float x, float y, float z)
        {
            // Convert the lower 4 bits of the hash into 12 gradient directions
            int h = hash & 15;
            float u = h < 8 ? x : y;
            float v = h < 4 ? y : h == 12 || h == 14 ? x : z;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }
    }
}
