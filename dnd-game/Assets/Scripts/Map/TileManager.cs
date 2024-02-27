using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }

    public Dictionary<TileType, Sprite[]> TileSprites = new();


    void Start()
    {
        // tilemap.
    }

    void Update()
    {

    }

    void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        // TileType.Unknown.
    }
}
