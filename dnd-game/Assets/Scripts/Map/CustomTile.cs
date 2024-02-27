
using UnityEngine;
using UnityEngine.Tilemaps;

public class CustomTile : Tile
{
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.colliderType = ColliderType.None;
        var tile = MapRenderer.Instance.Map.GetTileAt((Vector2Int)position);

    }
}