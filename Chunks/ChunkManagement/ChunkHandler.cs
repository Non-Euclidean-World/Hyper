using Chunks.MarchingCubes;
using Chunks.Voxels;
using Common.Meshes;
using OpenTK.Mathematics;

namespace Chunks.ChunkManagement;

public static class ChunkHandler
{
    public const string SaveLocation = "Chunks";

    // public static void SaveChunkData(Voxel[,,] voxels, Vector3i position)
    // {
    //     string filePath = GetFileName(position / Chunk.Size);
    //     SaveVoxels(filePath, voxels);
    // }
    //
    // public static Chunk LoadChunk(Vector3i position)
    // {
    //     string filePath = GetFileName(position);
    //     Voxel[,,] scalarField = LoadVoxels(filePath);
    //     var meshGenerator = new MeshGenerator(scalarField);
    //     Vertex[] data = meshGenerator.GetMesh();
    //
    //     return new Chunk(data, position * Chunk.Size, scalarField, false);
    // }
    //
    // private static void SaveVoxels(string filePath, Voxel[,,] voxels)
    // {
    //     using BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create));
    //     
    //     writer.Write(voxels.GetLength(0));
    //     writer.Write(voxels.GetLength(1));
    //     writer.Write(voxels.GetLength(2));
    //
    //     for (int i = 0; i < voxels.GetLength(0); i++)
    //     {
    //         for (int j = 0; j < voxels.GetLength(1); j++)
    //         {
    //             for (int k = 0; k < voxels.GetLength(2); k++)
    //             {
    //                 Voxel voxel = voxels[i, j, k];
    //                 writer.Write(voxel.Value);
    //                 writer.Write((int)voxel.Type);
    //             }
    //         }
    //     }
    // }
    //
    // private static Voxel[,,] LoadVoxels(string filePath)
    // {
    //     using BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open));
    //     
    //     int xLength = reader.ReadInt32();
    //     int yLength = reader.ReadInt32();
    //     int zLength = reader.ReadInt32();
    //
    //     var readVoxels = new Voxel[xLength, yLength, zLength];
    //     for (int i = 0; i < xLength; i++)
    //     {
    //         for (int j = 0; j < yLength; j++)
    //         {
    //             for (int k = 0; k < zLength; k++)
    //             {
    //                 float value = reader.ReadSingle();
    //                 VoxelType type = (VoxelType)reader.ReadInt32();
    //                 readVoxels[i, j, k] = new Voxel { Value = value, Type = type };
    //             }
    //         }
    //     }
    //
    //     return readVoxels;
    // }
    //
    // private static string GetFileName(Vector3i position)
    // {
    //     return $"{SaveLocation}/{position.X}_{position.Y}_{position.Z}.voxels";
    // }
}