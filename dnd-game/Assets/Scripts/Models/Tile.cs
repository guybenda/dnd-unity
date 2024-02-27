
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum TileType : int
{
    Unknown = -1,
    Empty = 0,
    CobblestoneA,
    CobblestoneB,
    FlatStoneFloorA,
    FlatStoneFloorB,
    GrassBright,
    GrassDarkRocky,
    GrassDark,
    Grass,
    GrayBrickFloorA,
    GrayBrickFloorB,
    GrayBrickFloorC,
    GrayBrickFloorD,
    HerringboneA,
    HerringboneB,
    HerringboneC,
    RectangularTilesA,
    RectangularTilesB,
    RectangularTilesC,
    RectangularTilesD,
    RectangularTilesE,
    RockTilesA,
    RockTilesB,
    RockTilesMossyA,
    RockTilesMossyB,
    RockTilesMossyC,
    SparseGrassBright,
    SparseGrassDark,
    SparseGrass,
    SquareCobblestoneA,
    SquareCobblestoneB,
    SquareCobblestoneC,
    SquareCobblestoneD,
    SquareCobblestoneE,
    SquareTilesA,
    SquareTilesB,
    SquareTilesC,
    SquareTilesD,
    SquareTilesE,
    SquareTilesF,
    UnevenBrickFloorA,
    UnevenBrickFloorB,
    UnevenBrickFloorC,
    UnevenBrickFloorD,
    UnevenBrickFloorE,
    UnevenBrickFloorF,
}

public enum TileTextureType
{
    Regular,
    Tiling,
    Random,
}

public static class TileTypeExtensions
{
    static readonly Dictionary<TileType, Sprite[]> spriteCache = new();

    public static TileTextureType TextureType(this TileType tileType)
    {
        switch (tileType)
        {
            case TileType.SquareTilesA:
            case TileType.SquareTilesB:
            case TileType.SquareTilesC:
            case TileType.SquareTilesD:
            case TileType.SquareTilesE:
            case TileType.SquareTilesF:
                return TileTextureType.Random;
        }

        return TileTextureType.Tiling;
    }

    public static Sprite[] Sprites(this TileType tileType)
    {
        if (spriteCache.TryGetValue(tileType, out var sprites))
        {
            return sprites;
        }

        sprites = Resources.LoadAll<Sprite>($"Tiles/{tileType}");
        spriteCache[tileType] = sprites;

        return sprites;
    }

    public static Sprite SpriteAt(this TileType tileType, Vector2Int position)
    {
        if (tileType == TileType.Empty) return null;

        var textureType = tileType.TextureType();
        var sprites = tileType.Sprites();

        switch (textureType)
        {
            case TileTextureType.Regular:
                return sprites[0];
            case TileTextureType.Tiling:
                {
                    var dim = (int)Math.Sqrt(sprites.Length);
                    var xIndex = Mod(position.x, dim);
                    var yIndex = position.y * dim;
                    var index = Mod(xIndex - yIndex, sprites.Length);
                    return sprites[index];
                }
            case TileTextureType.Random:
                {
                    const float randomMult = Mathf.PI;
                    var offset = (int)tileType * 13.37f;
                    var indexF = Mathf.PerlinNoise(position.x * randomMult + offset, position.y * randomMult + offset) * 137.51f;
                    var index = Mod((int)indexF, sprites.Length);
                    // var index = (int)Mathf.Clamp(indexF, 0, sprites.Length - 1);
                    return sprites[index];
                }
        }

        return sprites[0];
    }

    static int Mod(int n, int m)
    {
        return (n % m + m) % m;
    }
}

public static class TileTypes
{

}