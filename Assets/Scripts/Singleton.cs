using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Singleton<T> : MonoBehaviour where T : Component
{
    public static T instance;

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
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