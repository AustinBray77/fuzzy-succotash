using UnityEngine;
using System;

public class GameHandler : Singleton<GameHandler>
{
    private void Start()
    {
        SaveHandler.Instance.Initialize(); //Set up file path
        ControlsManager.Instance.Initialize(); //setup of input actions
        LevelHandler.Instance.Initialize(); //Level handler relies on controls being set to add callbacks for inputs
        MenuController.Instance.Initialize(); //This relies on previous initialization to set up menus

        for (int i = 0; i < LevelHandler.Instance.LevelReferences.Count; i++)
        {
            LevelHandler.Instance.LevelReferences[i].Initialize(i);
        }

        try
        {
            SaveHandler.Instance.Load();
        }
        catch (Exception e)
        {
            Debug.Log("Could not load save data, ERROR: " + e.Message);
        }
    }
}