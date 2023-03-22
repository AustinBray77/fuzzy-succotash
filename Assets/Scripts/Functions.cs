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

    public static void SetActiveAllObjects(GameObject[] gameObjects, bool state)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(state);
        }
    }

    public static T[] SwapArrayType<T, U>(U[] inputArr) where T : class
    {
        T[] outputArr = new T[inputArr.Length];

        for (int i = 0; i < inputArr.Length; i++)
        {
            outputArr[i] = inputArr[i] as T;
        }

        return outputArr;
    }
}
