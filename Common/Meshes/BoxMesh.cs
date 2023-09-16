﻿using OpenTK.Mathematics;

namespace Common.Meshes;
public static class BoxMesh
{
    public static Mesh Create(Vector3 size, Vector3 position)
    {
        Vertex[] vertices = new Vertex[CubeMesh.Vertices.Length];
        Array.Copy(CubeMesh.Vertices, vertices, CubeMesh.Vertices.Length);

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].X *= size.X;
            vertices[i].Y *= size.Y;
            vertices[i].Z *= size.Z;
        }

        Mesh mesh = new Mesh(vertices, position);

        return mesh;
    }

    public static Mesh Create(Vector3 size)
        => Create(size, Vector3.Zero);
}
