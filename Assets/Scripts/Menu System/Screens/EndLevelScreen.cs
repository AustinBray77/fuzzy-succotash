using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class EndLevelScreen : MonoBehaviour, IScreen
{
    public string Name { get => "EndLevelScreen"; }

    [SerializeField] private GameObject[] _screenElements;
    public GameObject[] ScreenElements { get => _screenElements; }

    [SerializeField] TMP_Text timerText;
    private const string timerPrefix = "Time: ";

    //Method for initializing the screen
    public void Initialize()
    {
        
    }

    //Method for loading the screen in
    public void Load()
    {
        timerText.text = timerPrefix + LevelHandler.Instance.CurrentLevelController.LevelCompletionTime.RoundToDecimalPlaces(2);
        ControlsManager.Instance.AddCallback(ControlsManager.Actions.respawn, OnPress_Respawn);
        ScreenElements.SetActiveAllObjects(true);
    }

    //Method for unloading the screen
    public void Unload()
    {
        ControlsManager.Instance.RemoveCallback(ControlsManager.Actions.respawn, OnPress_Respawn);
        ScreenElements.SetActiveAllObjects(false);
    }
    
    #region InputCallback methods
    private void OnPress_Respawn(InputAction.CallbackContext context)
    {
        OnClick_RestartStage();
    }
    #endregion

    #region Button_Methods
    public void OnClick_NextLevel()
    {
        LevelHandler.Instance.NextLevel();
        StartCoroutine(MenuController.Instance.OpenScreen("Game", false, false));
    }

    public void OnClick_RestartStage()
    {
        LevelHandler.Instance.CurrentLevelController.RespawnFromMenu();
        StartCoroutine(MenuController.Instance.OpenScreen("Game", false, false));
    }

    public void OnClick_RestartLevel()
    {
        LevelHandler.Instance.CurrentLevelController.RestartLevelFromMenu();
        StartCoroutine(MenuController.Instance.OpenScreen("Game", false, false));
    }

    public void OnClick_LevelSelect()
    {
        LevelHandler.Instance.UnloadLevel(); //Figure how to do this in the middle of the fade instead of right away
        StartCoroutine(MenuController.Instance.OpenScreen("LevelSelect", inputMapsOnFinish: ControlsManager.InputMap.menus));
    }
    #endregion

    //Add callbacks and add region for callback functions
}
