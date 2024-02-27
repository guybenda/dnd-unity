
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapRenderer : MonoBehaviour
{
    public static MapRenderer Instance { get; private set; }


    [Range(1, 25)]
    public int RenderDistanceChunks = 10;

    public Tilemap Tilemap;

    SyncExecutor exec = new();
    CustomTile customTileInstance;
    TileBase[] fillTileArray;
    TileBase[] emptyTileArray;


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

        fillTileArray = new TileBase[MapChunk.ChunkSize * MapChunk.ChunkSize];
        emptyTileArray = new TileBase[MapChunk.ChunkSize * MapChunk.ChunkSize];
        for (int i = 0; i < fillTileArray.Length; i++)
        {
            fillTileArray[i] = customTileInstance;
        }
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
        var camPos = Camera.main.transform.position;
        var cameraChunkPos = new Vector2(camPos.x / MapChunk.ChunkSizeF, camPos.z / MapChunk.ChunkSizeF);

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
        var fromY = chunkPos.y * MapChunk.ChunkSize;

        var bounds = new BoundsInt(new Vector3Int(fromX, fromY, 0), new Vector3Int(MapChunk.ChunkSize, MapChunk.ChunkSize, 1));

        Tilemap.SetTilesBlock(bounds, fillTileArray);
        // Tilemap.BoxFill(new Vector3Int(fromX, fromY, 0), customTileInstance, fromX, fromY, toX, toY);
    }

    void UnloadChunk(Vector2Int chunkPos)
    {
        var fromX = chunkPos.x * MapChunk.ChunkSize;
        var fromY = chunkPos.y * MapChunk.ChunkSize;

        var bounds = new BoundsInt(new Vector3Int(fromX, fromY, 0), new Vector3Int(MapChunk.ChunkSize, MapChunk.ChunkSize, 1));

        Tilemap.SetTilesBlock(bounds, emptyTileArray);
    }
}