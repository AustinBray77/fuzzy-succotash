using UnityEngine;

public class MainScreen : MonoBehaviour, IScreen
{
    public GameObject[] ScreenElements { get; set; }
    public string Name { get; set; }

    //Method for initializing the screen
    public void Initialize()
    {

    }

    //Method for loading the screen in
    public void Load()
    {
        //Enables each screen element
        foreach (GameObject element in ScreenElements)
        {
            element.SetActive(true);
        }
    }

    //Method for unloading the screen
    public void Unload()
    {
        //Disables each screen element
        foreach (GameObject element in ScreenElements)
        {
            element.SetActive(false);
        }
    }


}