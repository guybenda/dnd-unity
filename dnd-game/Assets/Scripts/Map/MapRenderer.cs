
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapRenderer : MonoBehaviour
{
    public static MapRenderer Instance { get; private set; }
    SyncExecutor exec = new();

    public Map Map;

    [Range(1, 25)]
    public int RenderDistanceChunks = 5;

    Tilemap tilemap;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        tilemap = GetComponentInChildren<Tilemap>();
    }

    void Start()
    {

    }

    void Update()
    {
        if (Map != null)
        {
            RenderMap();
        }

        exec.Execute(1);
    }

    void RenderMap()
    {
        var cameraPos = ((Vector2)Camera.main.transform.position) / MapChunk.ChunkSizeF;

        foreach (var chunkPos in Map.Chunks.Keys)
        {
            var distance = Vector2.Distance(cameraPos, chunkPos);
            if (distance > RenderDistanceChunks)
            {
                continue;
            }


        }
    }

    void LoadChunk(Vector2Int chunkPos)
    {
        if (!Map.Chunks.TryGetValue(chunkPos, out var chunk))
        {
            return;
        }

        // tilemap.BoxFill
    }
}