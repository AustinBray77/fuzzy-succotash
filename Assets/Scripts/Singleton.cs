using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Note: Use proper conventions: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions 

public class Singleton<T> : MonoBehaviour where T : Component
{
    public static T Instance;

    public virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            string locations = "";
            foreach (T component in FindObjectsOfType<T>(true))
            {
                locations += component.gameObject.name + ", ";
            }
            Debug.LogError("There can only be one " + typeof(T) + " because it is marked as a singleton\nThere are " + typeof(T) + "s on: " + locations);
        }
    }
}