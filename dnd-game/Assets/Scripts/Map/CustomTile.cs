
using UnityEngine;
using UnityEngine.Tilemaps;

public class CustomTile : Tile
{
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        var pos = (Vector2Int)position;
        tileData.colliderType = ColliderType.None;
        var tileType = MapManager.Instance.Map.GetTileAt(pos);

        tileData.sprite = tileType.SpriteAt(pos);
    }
}