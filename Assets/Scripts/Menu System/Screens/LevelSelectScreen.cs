using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectScreen : MonoBehaviour, IScreen
{
    private GameObject[] _screenElements;
    public GameObject[] ScreenElements { get => _screenElements; }
    public string Name { get => "LevelSelect"; }
    [SerializeField] private GameObject _levelCardPrefab;

    public void Initialize()
    {
        GenerateLevelCards();
    }

    public void Load()
    {
        Functions.SetActiveAllObjects(ScreenElements, true);
    }

    public void Unload()
    {
        Functions.SetActiveAllObjects(ScreenElements, false);
    }

    public void GenerateLevelCards()
    {

    }
}
