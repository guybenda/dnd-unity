
using UnityEngine;
using UnityEngine.Tilemaps;

public class CustomTile : Tile
{
    static readonly Matrix4x4 transformMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 180, 0));
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        var pos = (Vector2Int)position;
        tileData.colliderType = ColliderType.None;
        var tileType = MapManager.Instance.Map.GetTileAt(pos);

        tileData.sprite = tileType.SpriteAt(pos);

        // tileData.transform = transformMatrix;
    }
}