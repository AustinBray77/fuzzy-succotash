using UnityEngine;
using UnityEngine.InputSystem;

public class LevelHandler : Singleton<LevelHandler>
{
    //Levels
    public LevelController[] LevelReferences;

    //Current level
    public int CurrentLevelIndex;

    //Current Level Reference
    public LevelController CurrentLevelController;

    //Adds a callback to the respawn action to respawn the level when pressed
    private void Start()
    {
        ControlsManager.Instance.AddCallBack(ControlsManager.Actions.respawn, (InputAction.CallbackContext context) => Respawn(LevelController.RespawnInfo.manualRespawn));
    }

    //Function to load levels
    public void LoadLevel(int level)
    {
        //Spawns in the level and saves it
        CurrentLevelController = Instantiate(LevelReferences[level].gameObject, Vector3.zero, Quaternion.identity).GetComponent<LevelController>();

        //Starts the level
        CurrentLevelController.StartLevel();
    }

    //Function to Respawn the Level
    public void Respawn(LevelController.RespawnInfo info) =>
        CurrentLevelController.Respawn(info);

    //Function to load the next level
    public void NextLevel()
    {
        UnloadLevel();
        LoadLevel(++CurrentLevelIndex);
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
        /*** FUTURE CODING ***/
    }

    //Function for when the user closes the application 
    private void OnApplicationQuit() =>
        UnloadLevel();
}
