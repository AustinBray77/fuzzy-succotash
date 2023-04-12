using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour, IScreen
{
    public string Name { get => "PauseMenu"; }

    [SerializeField] private GameObject[] _screenElements;
    public GameObject[] ScreenElements { get => _screenElements; }

    //Method for initializing the screen
    public void Initialize()
    {

    }

    //Method for loading the screen in
    public void Load()
    {
        ScreenElements.SetActiveAllObjects(true);
        ControlsManager.Instance.AddCallback(ControlsManager.Actions.pause, OnPress_Resume);
        ControlsManager.Instance.AddCallback(ControlsManager.Actions.respawn, OnPress_Respawn);
    }

    //Method for unloading the screen
    public void Unload()
    {
        ScreenElements.SetActiveAllObjects(false);
        ControlsManager.Instance.RemoveCallback(ControlsManager.Actions.pause, OnPress_Resume);
        ControlsManager.Instance.RemoveCallback(ControlsManager.Actions.respawn, OnPress_Respawn);
    }

    #region InputCallback methods
    private void OnPress_Resume(InputAction.CallbackContext context)
    {
        OnClick_Resume();
    }

    private void OnPress_Respawn(InputAction.CallbackContext context)
    {
        OnClick_RestartStage();
    }
    #endregion

    #region Button_Methods
    public void OnClick_Resume()
    {
        StartCoroutine(OnClick_Resume_Coroutine());
    }

    private IEnumerator OnClick_Resume_Coroutine()
    {
        yield return StartCoroutine(MenuController.Instance.OpenScreen("Game", false, false));
        LevelHandler.Instance.CurrentLevelController.UnpauseFromPauseMenu();
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

    public void OnClick_Settings()
    {
        //Open settings menu
    }

    public void OnClick_LevelSelect()
    {
        LevelHandler.Instance.UnloadLevel(); //Figure how to do this in the middle of the fade instead of right away
        StartCoroutine(MenuController.Instance.OpenScreen("LevelSelect", inputMapsOnFinish: ControlsManager.InputMap.menus));
    }

    #endregion

    //Add callbacks and add region for callback functions
}
