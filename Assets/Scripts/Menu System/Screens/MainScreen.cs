using UnityEngine;

public class MainScreen : MonoBehaviour, IScreen
{
    [SerializeField] private GameObject[] _screenElements;
    public GameObject[] ScreenElements { get => _screenElements; }
    public string Name { get => "Main"; }

    //Method for initializing the screen
    public void Initialize()
    {

    }

    //Method for loading the screen in
    public void Load()
    {
        Functions.SetActiveAllObjects(ScreenElements, true);
    }

    //Method for unloading the screen
    public void Unload()
    {
        Functions.SetActiveAllObjects(ScreenElements, false);
    }

    #region Button_Methods
    public void OnClick_Start()
    {
        MenuController.Instance.OpenScreen("LevelSelect");
    }

    public void OnClick_Settings()
    {
        MenuController.Instance.OpenScreen("Settings");
    }

    public void OnClick_Quit()
    {
        Application.Quit();
    }
    #endregion
}