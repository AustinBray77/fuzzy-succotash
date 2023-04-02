using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameScreen : MonoBehaviour, IScreen
{
    [SerializeField] private GameObject[] _screenElements;
    [SerializeField] private TextMeshProUGUI _timerText;
    public GameObject[] ScreenElements { get => _screenElements; }

    public string Name { get => "Game"; }

    private bool _loaded = false;

    public void Initialize()
    {
        _loaded = false;
    }

    public void Load()
    {
        ScreenElements.SetActiveAllObjects(true);
        _loaded = true;
    }

    public void Unload()
    {
        ScreenElements.SetActiveAllObjects(false);
        _loaded = false;
    }

    private void Update()
    {
        if (!_loaded) return;

        _timerText.text = (Time.timeAsDouble - LevelHandler.Instance.CurrentLevelController.LevelStartTime)
            .RoundToDecimalPlaces(1).ToString() + "s";
    }
}
