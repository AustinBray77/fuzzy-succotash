using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.ObjectModel;

public class LevelHandler : Singleton<LevelHandler>
{
    //Levels (Changed to private so that other scripts cannot modify the levelControllers)
    [SerializeField] private LevelController[] levelReferences;
    public ReadOnlyCollection<LevelController> LevelReferences { get; private set; }

    //Current level
    public int CurrentLevelIndex { get; private set; }

    //Current Level Reference
    public LevelController CurrentLevelController { get; private set; }

    //Intializes all of the levels
    public void Initialize()
    {
        for (int i = 0; i < levelReferences.Length; i++)
        {
            Debug.Log("Initializing level:" + i);
            levelReferences[i].Initialize(i);
        }

        LevelReferences = new ReadOnlyCollection<LevelController>(levelReferences);
    }

    //Function to load levels
    public void LoadLevel(int level, int stage, float waitTimeBeforeStart)
    {
        
        //Spawns in the level and saves it
        CurrentLevelController = Instantiate(levelReferences[level].gameObject, Vector3.zero, Quaternion.identity).GetComponent<LevelController>();
        CurrentLevelController.OnSpawn();

        //Enables the player
        Player.Instance.SetActive(true);

        //Starts the level
        CurrentLevelController.StartLevel(stage, waitTimeBeforeStart);
    }

    /*
    //Function to Respawn the Level
    public void Respawn(LevelController.RespawnInfo info) =>
        CurrentLevelController.Respawn(info);

    private void PauseLevel()
    {
        CurrentLevelController.PausePressed();
    }    
    */

    //Function to load the next level
    public void NextLevel()
    {
        UnloadLevel();
        LoadLevel(++CurrentLevelIndex, 0, 0);
    }

    //Function to Unload a Level
    public void UnloadLevel()
    {
        SaveLevelData();
        Destroy(CurrentLevelController.gameObject);

        Player.Instance.SetActive(false);
    }

    //Function to Save the Level Data
    public void SaveLevelData()
    {
        SaveHandler.Instance.Save();
    }

    //For some reason this is being called on run
    //Function for when the user closes the application 
    private void OnApplicationQuit()
    {
        if (CurrentLevelController is not null)
        {
            UnloadLevel();
        }
    }
}
