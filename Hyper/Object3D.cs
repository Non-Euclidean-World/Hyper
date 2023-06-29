using Hyper.Meshes;
using OpenTK.Mathematics;

namespace Hyper
{
    public class Object3D
    {
        public List<Mesh> Meshes { get; set; }

        public Vector3 Position { get; set; } = Vector3.Zero;
        //Will also have to add rotation and scale

        public Object3D()
        {
            Meshes = new List<Mesh>();
        }

        public Object3D(List<Mesh> meshes)
        {
            Meshes = meshes;
        }

        public Object3D(Mesh mesh)
        {
            Meshes = new List<Mesh> { mesh };
        }
    }
}
