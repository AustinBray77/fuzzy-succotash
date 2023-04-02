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
        ScreenElements.SetActiveAllObjects(true);
    }

    //Method for unloading the screen
    public void Unload()
    {
        ScreenElements.SetActiveAllObjects(false);
    }

    #region Button_Methods
    public void OnClick_Start()
    {
        //Temp change to load test level
        //LevelHandler.Instance.LoadLevel(0, 0);
        //Unload();

        StartCoroutine(MenuController.Instance.OpenScreen("LevelSelect"));
    }

    public void OnClick_Settings()
    {
        StartCoroutine(MenuController.Instance.OpenScreen("Settings"));
    }

    public void OnClick_Quit()
    {
        Application.Quit();
    }
    #endregion
}