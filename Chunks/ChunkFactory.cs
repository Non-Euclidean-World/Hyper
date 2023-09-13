﻿using System.Collections.Concurrent;
using Chunks.MarchingCubes;
using Chunks.Voxels;
using Common.Meshes;
using NLog;
using OpenTK.Mathematics;

namespace Chunks;

public class ChunkFactory
{
    private readonly ScalarFieldGenerator _scalarFieldGenerator;

    private readonly Voxel[][,,] _voxelPool;

    public readonly BlockingCollection<int> FreeVoxels;

    public const string SaveLocation = "Chunks";

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public ChunkFactory(ScalarFieldGenerator scalarFieldGenerator, int renderDistance)
    {
        _scalarFieldGenerator = scalarFieldGenerator;
        int size = 2 * renderDistance + 1;
        _voxelPool = new Voxel[3 * size * size * size][,,];
        for (int i = 0; i < _voxelPool.Length; i++)
        {
            _voxelPool[i] = new Voxel[Chunk.Size + 1, Chunk.Size + 1, Chunk.Size + 1];
        }
        FreeVoxels = new BlockingCollection<int>(new ConcurrentQueue<int>());
        for (int i = 0; i < _voxelPool.Length; i++)
        {
            FreeVoxels.Add(i);
        }
    }

    public Chunk GenerateChunk(Vector3i position, bool generateVao = true)
    {
        int index = FreeVoxels.Take();
        _scalarFieldGenerator.Generate(Chunk.Size, position, _voxelPool[index]);
        var meshGenerator = new MeshGenerator(_voxelPool[index]);
        Vertex[] data = meshGenerator.GetMesh();

        if (FreeVoxels.Count == 0) Logger.Warn("FreeVoxels queue is empty!");

        return new Chunk(data, position, _voxelPool[index], index, generateVao);
    }

    public void SaveChunkData(int index, Vector3i position)
    {
        string filePath = GetFileName(position / Chunk.Size);
        SaveVoxels(filePath, _voxelPool[index]);
    }

    public Chunk LoadChunk(Vector3i position)
    {
        int index = FreeVoxels.Take();
        string filePath = GetFileName(position);
        LoadVoxels(filePath, _voxelPool[index]);
        var meshGenerator = new MeshGenerator(_voxelPool[index]);
        Vertex[] data = meshGenerator.GetMesh();

        if (FreeVoxels.Count == 0) Logger.Warn("FreeVoxels queue is empty!");

        return new Chunk(data, position * Chunk.Size, _voxelPool[index], index, false);
    }

    private static void SaveVoxels(string filePath, Voxel[,,] voxels)
    {
        using BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create));

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

    private static void LoadVoxels(string filePath, Voxel[,,] voxels)
    {
        using BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open));

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
    }

    private static string GetFileName(Vector3i position)
    {
        return $"{SaveLocation}/{position.X}_{position.Y}_{position.Z}.voxels";
    }
}
