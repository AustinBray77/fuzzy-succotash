using UnityEngine;
using System;

public class GameHandler : Singleton<GameHandler>
{
    private void Start()
    {
        SaveHandler.Instance.Initialize(); //Set up file path
        ControlsManager.Instance.Initialize(); //setup of input actions
        LevelHandler.Instance.Initialize();
        MenuController.Instance.Initialize(); //This relies on previous initialization to set up menus
        Player.Instance.Initialize();

        try
        {
            //SaveHandler.Instance.Load();
        }
        catch (Exception e)
        {
            Debug.Log("Could not load save data, ERROR: " + e.Message);
        }
    }
}