
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
        Map = new Map("test_game_id");
    }

    void Update()
    {

    }

    public void SetMap(Map map)
    {
        Map = map;
    }

    public void OnClick(Vector3 clickPosition)
    {
        Vector2Int position = new Vector2Int(
            (int)Math.Floor(clickPosition.x),
            (int)Math.Floor(clickPosition.z)
        );
        Debug.Log(position);

        Map.SetTileAt(position, TileType.CobblestoneA);
        MapRenderer.Instance.Tilemap.RefreshTile((Vector3Int)position);
    }
}