using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : Singleton<MenuController>
{
    //Array for all menu screens
    [SerializeField] private IScreen[] _screens;
    private Dictionary<string, IScreen> _screenLib = new Dictionary<string, IScreen>();

    //Method for on start
    private void Start()
    {
        //Loads the screens to the dictionary
        LoadScreens();
        _screenLib["Main"].Load();
    }

    //Loads all screens from the array into the dictionary
    private void LoadScreens()
    {
        //Loops through each screen and adds it to the dictionary with its name as the key and initializes it
        foreach (IScreen screen in _screens)
        {
            _screenLib.Add(screen.Name, screen);
            screen.Initialize();
            screen.Unload();
        }
    }
}
