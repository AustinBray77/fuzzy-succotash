using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public static class ExtensionMethods
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

    public static HashSet<T> GetAllComponents<T>(this GameObject[] gameObjects) where T : MonoBehaviour
    {
        HashSet<T> output = new HashSet<T>();

        foreach (GameObject gameObject in gameObjects)
        {
            T[] components = gameObject.GetComponentsInChildren<T>();

            foreach (T val in components)
            {
                if (!output.Contains(val))
                {
                    output.Add(val);
                }
            }
        }

        return output;
    }

    public static void SetActiveAllObjects(this GameObject[] gameObjects, bool state)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(state);
        }
    }

    public static T[] SwapArrayType<T, U>(this U[] inputArr) where T : class
    {
        T[] outputArr = new T[inputArr.Length];

        for (int i = 0; i < inputArr.Length; i++)
        {
            outputArr[i] = inputArr[i] as T;
        }

        return outputArr;
    }

    public static double RoundToDecimalPlaces(this double d, int decimalPlaces)
        => System.Math.Round(d * System.Math.Pow(10, decimalPlaces)) / System.Math.Pow(10, decimalPlaces);
}
