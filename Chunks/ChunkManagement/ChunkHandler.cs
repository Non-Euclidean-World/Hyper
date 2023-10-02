using Chunks.MarchingCubes;
using Chunks.Voxels;
using Common;
using Common.Meshes;
using OpenTK.Mathematics;

namespace Chunks.ChunkManagement;

public class ChunkHandler
{
    public struct ChunkData
    {
        public Voxel[,,] Voxels;
        public int SphereId;
    }

    private readonly string _saveLocation;

    private readonly Vector3i[] _sphereCenters; // TODO the number of places where we're passing these things is ATROCIOUS

    private readonly float _globalScale;

    public ChunkHandler(string saveName, Vector3i[] sphereCenters, float globalScale)
    {
        _saveLocation = Path.Combine(Settings.SavesLocation, saveName, "chunks");
        Directory.CreateDirectory(_saveLocation);
        _sphereCenters = sphereCenters;
        _globalScale = globalScale;
    }

    public void SaveChunkData(Vector3i position, ChunkData chunkData, bool spherical = false)
    {
        string filePath = spherical ? GetFileName(position) : GetFileName(position / Chunk.Size);
        SaveChunkData(filePath, chunkData);
    }

    public Chunk LoadChunk(Vector3i chunkId, bool spherical = false)
    {
        string filePath = GetFileName(chunkId);
        var chunkData = LoadChunkData(filePath);
        var meshGenerator = new MeshGenerator(chunkData.Voxels);
        Vector3i position = spherical ? chunkId : chunkId * Chunk.Size;
        Vertex[] data = spherical ? meshGenerator.GetSphericalMesh(position, _sphereCenters[chunkData.SphereId], _globalScale) : meshGenerator.GetMesh();
        return new Chunk(data, position, chunkData.Voxels, chunkData.SphereId, createVao: false);
    }

    public List<Vector3i> GetSavedChunks()
    {
        if (!Directory.Exists(_saveLocation))
            return new List<Vector3i>();

        return Directory.GetFiles(_saveLocation, "*.voxels")
            .Select(file => GetPositionFromName(Path.GetFileName(file))).ToList();
    }

    public List<Chunk> LoadAllSavedChunks(bool spherical)
    {
        List<Vector3i> savedChunkIds = GetSavedChunks();
        return savedChunkIds.Select((p) => LoadChunk(p, spherical)).ToList();
    }

    private static void SaveChunkData(string filePath, ChunkData chunkData)
    {
        using BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create));

        writer.Write(chunkData.SphereId);
        ref var voxels = ref chunkData.Voxels;

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

    private static ChunkData LoadChunkData(string filePath)
    {
        using BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open));

        int sphereId = reader.ReadInt32();

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

        return new ChunkData { Voxels = voxels, SphereId = sphereId };
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