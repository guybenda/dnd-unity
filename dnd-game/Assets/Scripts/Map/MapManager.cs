
using System;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    public Map Map { get; private set; }

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        Map = Map.NewDefaultMap("test_game_id");
    }

    void Update()
    {

    }

    public void SetMap(Map map)
    {
        Map = map;
    }

    public void DrawTile(Vector3 clickPosition, TileType tileType)
    {
        var position = (Vector2Int)MapRenderer.Instance.Tilemap.WorldToCell(clickPosition);

        if (Map.SetTileAt(position, tileType))
        {
            MapRenderer.Instance.Tilemap.RefreshTile((Vector3Int)position);
        }
    }
}