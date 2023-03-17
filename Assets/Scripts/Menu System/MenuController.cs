using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : Singleton<MenuController>
{
    private Dictionary<string, IScreen> _screenLib = new Dictionary<string, IScreen>();
    private string _currentScreenName;

    [Serializable]
    public class UIPrefabs
    {
        public GameObject LevelCard;
        public GameObject Star;
    }

    public UIPrefabs Prefabs;

    //Method for on start
    private void Start()
    {
        //Loads the screens to the dictionary
        InitializeScreens();
        OpenScreen("Main");
    }

    //Loads all screens from the array into the dictionary
    private void InitializeScreens()
    {
        IScreen[] screens = GetComponentsInChildren<IScreen>();

        Debug.Log(screens.Length);


        //Loops through each screen and adds it to the dictionary with its name as the key and initializes it
        foreach (IScreen screen in screens)
        {
            _screenLib.Add(screen.Name, screen);
            screen.Initialize();
            screen.Unload();
        }
    }

    //Method to load a given screen using the name
    public void OpenScreen(string screenName)
    {
        /*** Add in animation here later ***/

        if (!string.IsNullOrEmpty(_currentScreenName))
        {
            _screenLib[_currentScreenName].Unload();
        }

        _currentScreenName = screenName;
        _screenLib[screenName].Load();
    }
}
