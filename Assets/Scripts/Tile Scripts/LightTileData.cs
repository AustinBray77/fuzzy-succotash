using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu]
public class LightTileData : ScriptableObject
{
    public TileBase[] tiles;
    public Light2D Light;
}