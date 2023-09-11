using System.Collections.Concurrent;
using Chunks.MarchingCubes;
using Chunks.Voxels;
using Common.Meshes;
using OpenTK.Mathematics;

namespace Chunks;

public class ChunkFactory
{
    private readonly ScalarFieldGenerator _scalarFieldGenerator;

    private readonly Voxel[][,,] _voxelPool;
    
    public readonly ConcurrentQueue<int> FreeVoxels;
    
    private const string SaveLocation = "Chunks";

    public ChunkFactory(ScalarFieldGenerator scalarFieldGenerator, int renderDistance)
    {
        _scalarFieldGenerator = scalarFieldGenerator;
        int size = 2 * renderDistance + 1;
        _voxelPool = new Voxel[3 * size * size * size][,,]; // 3 * should work but maybe we can do better.
        for (int i = 0; i < _voxelPool.Length; i++)
        {
            _voxelPool[i] = new Voxel[Chunk.Size + 1, Chunk.Size + 1, Chunk.Size + 1];
        }
        FreeVoxels = new ConcurrentQueue<int>();
        for (int i = 0; i < _voxelPool.Length; i++)
        {
            FreeVoxels.Enqueue(i);
        }
    }

    public Chunk GenerateChunk(Vector3i position, bool generateVao = true)
    {
        FreeVoxels.TryDequeue(out int index);
        _scalarFieldGenerator.Generate(Chunk.Size, position, _voxelPool[index]);
        var meshGenerator = new MeshGenerator(_voxelPool[index]);
        Vertex[] data = meshGenerator.GetMesh();

        return new Chunk(data, position, _voxelPool[index], index, generateVao);
    }
    
    public void SaveChunkData(int index, Vector3i position)
    {
        string filePath = GetFileName(position / Chunk.Size);
        SaveVoxels(filePath, _voxelPool[index]);
    }

    public Chunk LoadChunk(Vector3i position)
    {
        FreeVoxels.TryDequeue(out int index);
        string filePath = GetFileName(position);
        LoadVoxels(filePath, _voxelPool[index]);
        var meshGenerator = new MeshGenerator(_voxelPool[index]);
        Vertex[] data = meshGenerator.GetMesh();

        return new Chunk(data, position * Chunk.Size, _voxelPool[index], index, false);
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

    private static void LoadVoxels(string filePath, Voxel[,,] readVoxels)
    {
        using BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open));
        
        // TODO Check if dimensions match
        int xLength = reader.ReadInt32();
        int yLength = reader.ReadInt32();
        int zLength = reader.ReadInt32();

        for (int i = 0; i < xLength; i++)
        {
            for (int j = 0; j < yLength; j++)
            {
                for (int k = 0; k < zLength; k++)
                {
                    float value = reader.ReadSingle();
                    VoxelType type = (VoxelType)reader.ReadInt32();
                    readVoxels[i, j, k] = new Voxel { Value = value, Type = type };
                }
            }
        }
    }
    
    private static string GetFileName(Vector3i position)
    {
        return $"{SaveLocation}/{position.X}_{position.Y}_{position.Z}.voxels";
    }
}
