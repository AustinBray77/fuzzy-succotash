using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreen : MonoBehaviour, IScreen
{
    [SerializeField] private GameObject[] _screenElements;
    public GameObject[] ScreenElements { get => _screenElements; }

    public string Name { get => "Game"; }

    public void Initialize()
    {

    }

    public void Load()
    {
        Functions.SetActiveAllObjects(ScreenElements, true);
    }

    public void Unload()
    {
        Functions.SetActiveAllObjects(ScreenElements, false);
    }
}
