
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapRenderer : MonoBehaviour
{
    public static MapRenderer Instance { get; private set; }


    [Range(1, 25)]
    public int RenderDistanceChunks = 5;

    SyncExecutor exec = new();
    public Tilemap Tilemap;
    CustomTile customTileInstance;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Tilemap = GetComponentInChildren<Tilemap>();
        customTileInstance = ScriptableObject.CreateInstance<CustomTile>();
    }

    void Start()
    {

    }

    void Update()
    {
        if (MapManager.Instance.Map != null)
        {
            RenderMap();
        }

        exec.Execute(1);
    }

    void RenderMap()
    {
        var cameraChunkPos = ((Vector2)Camera.main.transform.position) / MapChunk.ChunkSizeF;

        foreach (var (chunkPos, chunk) in MapManager.Instance.Map.Chunks)
        {
            var distance = Vector2.Distance(cameraChunkPos, chunkPos);
            if (distance > RenderDistanceChunks)
            {
                if (chunk.IsRendered)
                {
                    UnloadChunk(chunkPos);
                    chunk.IsRendered = false;
                }

                continue;
            }

            if (!chunk.IsRendered)
            {
                LoadChunk(chunkPos);
                chunk.IsRendered = true;
            }

        }
    }

    void LoadChunk(Vector2Int chunkPos)
    {
        var fromX = chunkPos.x * MapChunk.ChunkSize;
        var toX = fromX + MapChunk.ChunkSize;
        var fromY = chunkPos.y * MapChunk.ChunkSize;
        var toY = fromY + MapChunk.ChunkSize;

        Tilemap.BoxFill(new Vector3Int(fromX, fromY, 0), customTileInstance, fromX, fromY, toX, toY);
    }

    void UnloadChunk(Vector2Int chunkPos)
    {
        var fromX = chunkPos.x * MapChunk.ChunkSize;
        var toX = fromX + MapChunk.ChunkSize;
        var fromY = chunkPos.y * MapChunk.ChunkSize;
        var toY = fromY + MapChunk.ChunkSize;

        Tilemap.BoxFill(new Vector3Int(fromX, fromY, 0), null, fromX, fromY, toX, toY);
    }
}