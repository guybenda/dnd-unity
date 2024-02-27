
using System;
using System.Collections.Generic;
using Firebase.Firestore;
using UnityEngine;

public class MapChunk
{
    public const int ChunkSize = 16;
    public const float ChunkSizeF = ChunkSize;

    public TileType[][] TileData { get; set; }

    public bool[][] VisionData { get; set; }

    public bool IsRendered = false;

    void Init()
    {
        TileData = new TileType[ChunkSize][];
        VisionData = new bool[ChunkSize][];

        for (int i = 0; i < ChunkSize; i++)
        {
            TileData[i] = new TileType[ChunkSize];
            VisionData[i] = new bool[ChunkSize];
        }
    }

    public MapChunk()
    {
        Init();
    }

    public MapChunk(MapChunkFirebase dbChunk)
    {
        Init();

        for (int x = 0; x < ChunkSize; x++)
        {
            for (int y = 0; y < ChunkSize; y++)
            {
                TileData[x][y] = (TileType)dbChunk.TileData[x * ChunkSize + y];
                VisionData[x][y] = dbChunk.VisionData[x * ChunkSize + y];
            }
        }
    }
}

[FirestoreData]
public class MapChunkFirebase
{
    [FirestoreProperty]
    public int[] TileData { get; set; }

    [FirestoreProperty]
    public bool[] VisionData { get; set; }

}

public class Chunker<TData> : Dictionary<Vector2Int, TData> where TData : new()
{
    public const int ChunkSize = 16;
    public const float ChunkSizeF = ChunkSize;

    Vector2Int GetChunkPosition(Vector2Int position)
    {
        return new Vector2Int(
            (int)Math.Floor(position.x / MapChunk.ChunkSizeF),
            (int)Math.Floor(position.y / MapChunk.ChunkSizeF)
        );
    }

    public TData Get(Vector2Int position)
    {
        var chunkPosition = GetChunkPosition(position);

        if (!TryGetValue(chunkPosition, out var chunk))
        {
            return default;
        }

        return chunk;
    }

    public TData GetCreate(Vector2Int position)
    {
        var chunkPosition = GetChunkPosition(position);

        if (!TryGetValue(chunkPosition, out var chunk))
        {
            chunk = new TData();
            Add(chunkPosition, chunk);
        }

        return chunk;
    }
}