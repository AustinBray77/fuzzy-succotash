using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileData
{
    public float X { get; private set; }
    public float Y { get; private set; }
    public TileBase Tile { get; private set; }

    public TileData(float x, float y, TileBase tile)
    {
        X = x;
        Y = y;
        Tile = tile;
    }
}
