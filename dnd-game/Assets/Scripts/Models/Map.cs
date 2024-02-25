
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DndFirebase;
using Firebase.Firestore;
using Palmmedia.ReportGenerator.Core.Reporting.Builders.Rendering;
using Unity.Netcode;
using UnityEngine;

public class Map
{
    public string Id { get; set; }
    public string GameId { get; set; }
    public string Name { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Dictionary<Vector2Int, MapChunk> Chunks { get; set; }

    public Map(string gameId)
    {
        Id = Guid.NewGuid().ToString();
        GameId = gameId;
    }

    Map(MapFirebase dbMap)
    {
        Id = dbMap.Id;
        Name = dbMap.Name;
        GameId = dbMap.GameId;
        CreatedAt = dbMap.CreatedAt.ToDateTime();
        UpdatedAt = dbMap.UpdatedAt.ToDateTime();
    }

    public static async Task<Map> Get(string id)
    {
        var dbMap = await MapFirebase.Get(id);
        return new Map(dbMap);
    }

    public static async Task<List<Map>> GetGameMaps(string gameId)
    {
        var dbMaps = await MapFirebase.GetGameMaps(gameId);
        return dbMaps.Select(dbGame => new Map(dbGame)).ToList();
    }

    public async Task LoadData()
    {
        Chunks = new();
        var dbData = await MapFirebase.LoadData(Id);

        foreach (var (position, dbChunk) in dbData)
        {
            Chunks.Add(position, new MapChunk(dbChunk));
        }
    }

    public async Task Save()
    {
        var dbMap = new MapFirebase
        {
            Id = Id,
            Name = Name,
            GameId = GameId,
            CreatedAt = Timestamp.FromDateTime(CreatedAt),
            UpdatedAt = Timestamp.GetCurrentTimestamp(),
        };

        await dbMap.Save();
    }

    public async Task SaveData()
    {
        Dictionary<Vector2Int, MapChunkFirebase> convertedData = new(Chunks.Count);

        foreach (var (position, chunk) in Chunks)
        {
            uint[] tileData = new uint[MapChunk.ChunkSize * MapChunk.ChunkSize];
            bool[] visionData = new bool[MapChunk.ChunkSize * MapChunk.ChunkSize];
            for (int x = 0; x < MapChunk.ChunkSize; x++)
            {
                for (int y = 0; y < MapChunk.ChunkSize; y++)
                {
                    tileData[x * MapChunk.ChunkSize + y] = (uint)chunk.TileData[x][y];
                    visionData[x * MapChunk.ChunkSize + y] = chunk.VisionData[x][y];
                }
            }

            var dbChunk = new MapChunkFirebase
            {
                TileData = tileData,
                VisionData = visionData,
            };

            convertedData.Add(position, dbChunk);
        }

        await Save();
        await MapFirebase.SaveData(Id, convertedData);

    }
}


[FirestoreData]
class MapFirebase
{
    const string CollectionName = "maps";
    const string ChunkCollectionName = "chunks";

    public string Id { get; set; }

    [FirestoreProperty]
    public string OwnerEmail { get; set; }

    [FirestoreProperty]
    public string GameId { get; set; }

    [FirestoreProperty]
    public string Name { get; set; }

    [FirestoreProperty]
    public Timestamp CreatedAt { get; set; }

    [FirestoreProperty]
    public Timestamp UpdatedAt { get; set; }

    public

    static CollectionReference Col()
    {
        return DndFirestore.Instance.Db.Collection(CollectionName);
    }

    public static async Task<List<MapFirebase>> GetGameMaps(string gameId)
    {
        var results = await Col()
            .WhereEqualTo("OwnerEmail", AuthManager.Instance.CurrentUser.Email.ToString())
            .WhereEqualTo("GameId", gameId)
            .OrderBy("UpdatedAt")
            .GetSnapshotAsync();

        var maps = results.Documents.Select(doc =>
        {
            var map = doc.ConvertTo<MapFirebase>();
            map.Id = doc.Id;
            return map;
        }).ToList();

        return maps;
    }

    public static async Task<MapFirebase> Get(string id)
    {
        var doc = await Col().Document(id).GetSnapshotAsync();
        var map = doc.ConvertTo<MapFirebase>();
        map.Id = doc.Id;
        return map;
    }

    public static async Task<Dictionary<Vector2Int, MapChunkFirebase>> LoadData(string mapId)
    {
        Dictionary<Vector2Int, MapChunkFirebase> chunks = new();

        var results = await Col().Document(mapId).Collection(ChunkCollectionName).GetSnapshotAsync();
        foreach (var dbChunk in results.Documents)
        {
            var chunk = dbChunk.ConvertTo<MapChunkFirebase>();
            var chunkPositionParts = dbChunk.Id.Split(',');
            Vector2Int chunkPosition = new(int.Parse(chunkPositionParts[0]), int.Parse(chunkPositionParts[1]));

            chunks.Add(chunkPosition, chunk);
        }

        return chunks;
    }

    public async Task Save()
    {
        OwnerEmail = AuthManager.Instance.CurrentUser.Email.ToString();
        await Col().Document(Id).SetAsync(this);
    }

    public static async Task SaveData(string id, Dictionary<Vector2Int, MapChunkFirebase> data)
    {
        const int maxBatchSize = 100;

        var colRef = Col().Document(id).Collection(ChunkCollectionName);

        var batch = DndFirestore.Instance.Db.StartBatch();
        int count = 0;
        foreach (var (position, chunk) in data)
        {
            batch.Set(colRef.Document($"{position.x},{position.y}"), chunk);
            count++;

            if (count >= maxBatchSize)
            {
                await batch.CommitAsync();
                batch = DndFirestore.Instance.Db.StartBatch();
                count = 0;
            }
        }

        await batch.CommitAsync();
    }
}

public class MapChunk
{
    public const int ChunkSize = 16;

    public TileType[][] TileData { get; set; }

    public bool[][] VisionData { get; set; }

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
    public uint[] TileData { get; set; }

    [FirestoreProperty]
    public bool[] VisionData { get; set; }

}