using Chunks.MarchingCubes;
using Chunks.Voxels;
using Common;
using Common.Meshes;
using OpenTK.Mathematics;

namespace Chunks.ChunkManagement;

public class ChunkHandler
{
    private readonly string _saveLocation;

    public ChunkHandler(string saveName)
    {
        _saveLocation = Path.Combine(Settings.SavesLocation, saveName, "chunks");
        Directory.CreateDirectory(_saveLocation);
    }

    public void SaveChunkData(Voxel[,,] voxels, Vector3i position)
    {
        string filePath = GetFileName(position / Chunk.Size);
        SaveVoxels(filePath, voxels);
    }

    public Chunk LoadChunk(Vector3i position)
    {
        string filePath = GetFileName(position);
        var voxels = LoadVoxels(filePath);
        var meshGenerator = new MeshGenerator(voxels);
        Vertex[] data = meshGenerator.GetMesh();

        return new Chunk(data, position * Chunk.Size, voxels, false);
    }

    public List<Vector3i> GetSavedChunks()
    {
        if (!Directory.Exists(_saveLocation)) return new List<Vector3i>();
        
        return Directory.GetFiles(_saveLocation, "*.voxels")
            .Select(file => GetPositionFromName(Path.GetFileName(file))).ToList();
    }

    private static void SaveVoxels(string filePath, Voxel[,,] voxels)
    {
        using BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create));

        writer.Write(voxels.GetLength(0));
        writer.Write(voxels.GetLength(1));
        writer.Write(voxels.GetLength(2));

        for (int i = 0; i < voxels.GetLength(0); i++)
        {
            for (int j = 0; j < voxels.GetLength(1); j++)
            {
                for (int k = 0; k < voxels.GetLength(2); k++)
                {
                    Voxel voxel = voxels[i, j, k];
                    writer.Write(voxel.Value);
                    writer.Write((int)voxel.Type);
                }
            }
        }
    }

    private static Voxel[,,] LoadVoxels(string filePath)
    {
        using BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open));

        int xLength = reader.ReadInt32();
        int yLength = reader.ReadInt32();
        int zLength = reader.ReadInt32();

        var voxels = new Voxel[xLength, yLength, zLength];
        for (int i = 0; i < voxels.GetLength(0); i++)
        {
            for (int j = 0; j < voxels.GetLength(1); j++)
            {
                for (int k = 0; k < voxels.GetLength(2); k++)
                {
                    float value = reader.ReadSingle();
                    VoxelType type = (VoxelType)reader.ReadInt32();
                    voxels[i, j, k] = new Voxel { Value = value, Type = type };
                }
            }
        }

        return voxels;
    }

    private string GetFileName(Vector3i position)
    {
        return $"{_saveLocation}/{position.X}_{position.Y}_{position.Z}.voxels";
    }

    private static Vector3i GetPositionFromName(string fileName)
    {
        string[] split = fileName.Substring(0, fileName.IndexOf('.')).Split('_');
        return new Vector3i(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));
    }
}