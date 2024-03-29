
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DndFirebase;
using Firebase.Firestore;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map
{
    public string Id { get; set; }
    public string GameId { get; set; }
    public string Name { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Chunker<MapChunk> Chunks { get; set; } = new();

    public Map(string gameId, string name = "New Map")
    {
        Id = Guid.NewGuid().ToString();
        GameId = gameId;
        CreatedAt = DateTime.Now;
        Name = name;
    }

    public static Map NewDefaultMap(string gameId)
    {
        var map = new Map(gameId);

        for (int i = -10; i < 20; i++)
        {
            for (int j = -10; j < 20; j++)
            {
                map.SetTileAt(new Vector2Int(i, j), TileType.GrassDarkRocky);
            }
        }

        return map;
    }

    Map(MapFirebase dbMap)
    {
        Id = dbMap.Id;
        Name = dbMap.Name;
        GameId = dbMap.GameId;
        CreatedAt = dbMap.CreatedAt.ToDateTime();
        UpdatedAt = dbMap.UpdatedAt.ToDateTime();
    }

    public static async Task<Map> Get(string id, bool withData = false)
    {
        var dbMap = await MapFirebase.Get(id);
        var map = new Map(dbMap);

        if (withData)
        {
            await map.LoadData();
        }

        return map;
    }

    public static async Task<List<Map>> GetGameMaps(string gameId)
    {
        var dbMaps = await MapFirebase.GetGameMaps(gameId);
        return dbMaps.Select(dbGame => new Map(dbGame)).ToList();
    }

    public async Task<Map> LoadData()
    {
        Chunks = new();
        var dbData = await MapFirebase.LoadData(Id);

        foreach (var (position, dbChunk) in dbData)
        {
            Chunks.Add(position, new MapChunk(dbChunk));
        }

        return this;
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
            int[] tileData = new int[MapChunk.ChunkSize * MapChunk.ChunkSize];
            bool[] visionData = new bool[MapChunk.ChunkSize * MapChunk.ChunkSize];
            for (int x = 0; x < MapChunk.ChunkSize; x++)
            {
                for (int y = 0; y < MapChunk.ChunkSize; y++)
                {
                    tileData[x * MapChunk.ChunkSize + y] = (int)chunk.TileData[x][y];
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

    public TileType GetTileAt(Vector2Int position)
    {
        var chunk = Chunks.Get(position);
        if (chunk == null)
        {
            return TileType.Empty;
        }

        var localPosition = new Vector2Int(Mod(position.x), Mod(position.y));
        return chunk.TileData[localPosition.x][localPosition.y];
    }

    public bool SetTileAt(Vector2Int position, TileType tile)
    {
        var chunk = Chunks.GetCreate(position);

        var x = Mod(position.x);
        var y = Mod(position.y);

        var prev = chunk.TileData[x][y];
        chunk.TileData[x][y] = tile;

        return prev != tile;
    }

    public bool GetVisionAt(Vector2Int position)
    {
        var chunk = Chunks.Get(position);
        if (chunk == null)
        {
            return false;
        }

        var localPosition = new Vector2Int(Mod(position.x), Mod(position.y));
        return chunk.VisionData[localPosition.x][localPosition.y];
    }

    public void SetVisionAt(Vector2Int position, bool visible)
    {
        var chunk = Chunks.GetCreate(position);

        chunk.VisionData[Mod(position.x)][Mod(position.y)] = visible;
    }

    int Mod(int n)
    {
        return (n % MapChunk.ChunkSize + MapChunk.ChunkSize) % MapChunk.ChunkSize;
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
