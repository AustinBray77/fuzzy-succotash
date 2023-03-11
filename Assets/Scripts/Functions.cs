using UnityEngine;
using System.Collections.Generic;

public class Functions
{
    public static HashSet<T> GetAllComponents<T>(GameObject[] gameObjects) where T : MonoBehaviour
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
}
