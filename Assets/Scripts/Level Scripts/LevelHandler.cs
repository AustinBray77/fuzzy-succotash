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

    //Adds a callback to the respawn action to respawn the level when pressed
    public void Initialize()
    {
        ControlsManager.Instance.AddCallBack(ControlsManager.Actions.respawn, (InputAction.CallbackContext context) => Respawn(LevelController.RespawnInfo.manualRespawn));
        LevelReferences = new ReadOnlyCollection<LevelController>(levelReferences);
    }

    //Function to load levels
    public void LoadLevel(int level, int stage)
    {
        //Spawns in the level and saves it
        CurrentLevelController = Instantiate(levelReferences[level].gameObject, Vector3.zero, Quaternion.identity).GetComponent<LevelController>();

        //Starts the level
        CurrentLevelController.StartLevel(stage);
    }

    //Function to Respawn the Level
    public void Respawn(LevelController.RespawnInfo info) =>
        CurrentLevelController.Respawn(info);

    //Function to load the next level
    public void NextLevel()
    {
        UnloadLevel();
        LoadLevel(++CurrentLevelIndex, 0);
    }

    //Function to Unload a Level
    public void UnloadLevel()
    {
        SaveLevelData();
        Destroy(CurrentLevelController.gameObject);
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
        if(CurrentLevelController is not null)
        {
            UnloadLevel();
        }
    }
}
