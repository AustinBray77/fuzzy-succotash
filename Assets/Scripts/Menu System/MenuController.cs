using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class MenuController : Singleton<MenuController>
{
    private Dictionary<string, IScreen> _screenLib = new Dictionary<string, IScreen>();
    private string _currentScreenName;

    [Serializable]
    public class UIPrefabs
    {
        [SerializeField] public GameObject LevelCard;
        [SerializeField] public GameObject Star;
    }

    [SerializeField] public UIPrefabs Prefabs;
    [SerializeField] private InputSystemUIInputModule _inputSystem;

    //Method for on start
    public void Initialize()
    {
        //Loads the screens to the dictionary
        InitializeScreens();
        StartCoroutine(OpenScreen("Main"));
    }

    //Loads all screens from the array into the dictionary
    private void InitializeScreens()
    {
        IScreen[] screens = GetComponentsInChildren<IScreen>(true);

        //Loops through each screen and adds it to the dictionary with its name as the key and initializes it
        foreach (IScreen screen in screens)
        {
            _screenLib.Add(screen.Name, screen);
            if (screen is Component component)
            {
                component.gameObject.SetActive(true);
            }
            screen.Initialize();
            screen.Unload();
        }
    }

    //Method to load a given screen using the name
    public IEnumerator OpenScreen(string screenName, bool fadeIn = true, bool fadeOut = true)
    {
        _inputSystem.enabled = false;
        ControlsManager.Instance.DisableInput();

        if (fadeOut)
        {
            yield return StartCoroutine(AnimationManager.Instance.FadeTo(new Color(0, 0, 0)));
        }

        if (!string.IsNullOrEmpty(_currentScreenName))
        {
            _screenLib[_currentScreenName].Unload();
        }

        _currentScreenName = screenName;
        _screenLib[screenName].Load();

        if (fadeIn)
        {
            yield return StartCoroutine(AnimationManager.Instance.FadeIn());
        }

        ControlsManager.Instance.SetInputMaps(ControlsManager.InputMap.menus);
        
        _inputSystem.enabled = true;
    }
}
