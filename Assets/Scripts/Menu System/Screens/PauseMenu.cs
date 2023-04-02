using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    }

    //Method for unloading the screen
    public void Unload()
    {
        ScreenElements.SetActiveAllObjects(false);
    }

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
        StartCoroutine(MenuController.Instance.OpenScreen("LevelSelect"));
    }

    #endregion

    //Add callbacks and add region for callback functions
}
