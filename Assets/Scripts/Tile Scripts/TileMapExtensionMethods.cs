using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TileMapExtensionMethods
{
    public static IEnumerable<TileData> GetAllTiles(this Tilemap tilemap)
    {
        var bounds = tilemap.cellBounds;

        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y);
                Sprite sprite = tilemap.GetSprite(cellPosition);
                TileBase tile = tilemap.GetTile(cellPosition);

                if (tile == null && sprite == null)
                {
                    continue;
                }

                yield return new TileData(x, y, tile);
            }
        }
    }
}
