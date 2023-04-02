using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Tilemap))]
public class LightMapController : MonoBehaviour
{
    [SerializeField] private LightTileData[] _lightTileDatas;
    private Dictionary<TileBase, LightTileData> _lightTileDictionary;

    public void Initialize()
    {
        _lightTileDictionary = new Dictionary<TileBase, LightTileData>();

        foreach (LightTileData lightTileData in _lightTileDatas)
        {
            foreach (TileBase tile in lightTileData.tiles)
            {
                _lightTileDictionary.Add(tile, lightTileData);
            }
        }

        Tilemap tileMap = GetComponent<Tilemap>();

        foreach (TileData tile in tileMap.GetAllTiles())
        {
            LightTileData lightData = _lightTileDictionary[tile.Tile];
            Light2D light = Instantiate(lightData.Light, transform);
            light.transform.localPosition = new Vector3(tile.X + 0.5f, tile.Y + 0.5f);
        }
    }
}
