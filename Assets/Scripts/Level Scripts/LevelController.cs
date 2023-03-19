using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public bool LevelRunning { get; private set; } = false;
    [SerializeField] Transform playerStartPos;

    private LevelData _data;
    public LevelData Data { get => _data; }

    [SerializeField] private int _id;
    private int _numberOfStages;
    [SerializeField] private string _title;
    [SerializeField] private LevelProgresser levelChanges;
    [SerializeField] private float minHeight;

    private int currentStage = 0;

    private double levelStartTime;

    private void Update()
    {
        if(Player.Instance.PlayerPos.y < minHeight && !Player.Instance.Data.RespawningState)
        {
            Respawn(RespawnInfo.playerDied);
        }
    }

    //Called when the game loads, called once per run
    public void Initialize(int index)
    {
        //Set up the level Progressor so that it can easily move through stages later on
        levelChanges.Initialize();

        //Number of stages is calculated based on the serialized fields in the inspector
        _numberOfStages = levelChanges.NumberOfStages;

        //Reinitialize the data
        _data = new LevelData("level_" + _id, _numberOfStages, _title, index);
        SaveHandler.Instance.AddSaveableComponent(_data);
    }

    // Starts the level
    public void StartLevel(int stage)
    {
        ControlsManager.Instance.DisableInputMap(ControlsManager.InputMap.gameplay); //change to set to pause later
        currentStage = stage;
        levelChanges.LoadStage(stage);

        SpawnPlayer();
    }

    // Respawns the player
    public void Respawn(RespawnInfo info)
    {
        ControlsManager.Instance.DisableInputMap(ControlsManager.InputMap.gameplay); //change to set to pause later
        _data.LogRespawn(currentStage, info);

        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        
        Player.Instance.Data.RespawningState = true;
        //Fade out
        if (!AnimationManager.Instance.FadeToColour(Color.black, SpawnPlayerPart2))
        {
            Debug.LogWarning("Could not start fade out");
        }
    }

    private void SpawnPlayerPart2()
    {
        Player.Instance.SetTransform(playerStartPos);
        Player.Instance.Movement.ResetMovement();

        ResetLevel();

        //Fade in
        if (!AnimationManager.Instance.FadeFromColour(StartRun))
        {
            Debug.LogWarning("Could not start fade in");
        }
    }

    private void StartRun()
    {
        _data.LogAttemptStart(currentStage);

        //Turn off invincibility
        Player.Instance.Data.RespawningState = false;

        //Enable gameplay inputmap
        ControlsManager.Instance.SetInputMaps(ControlsManager.InputMap.gameplay);

        levelStartTime = Time.timeAsDouble;
        Time.timeScale = 1;

        //Tell the timer UI to start
    }

    private void ResetLevel()
    {
        levelChanges.LoadStage(currentStage);
    }

    public void PauseLevel()
    {
        Time.timeScale = 0;
        ControlsManager.Instance.SetInputMaps(ControlsManager.InputMap.menus);
        //Open pause menu?
    }

    public void LevelCompleted()
    {
        Player.Instance.Data.RespawningState = true;

        //Pause level and bring up level completion menu?
        Time.timeScale = 0;
        ControlsManager.Instance.SetInputMaps(ControlsManager.InputMap.menus);

        double time = Time.timeAsDouble - levelStartTime;

        _data.LogLevelCompletion(currentStage, time);
    }

    private void OnDestroy()
    {

    }

    public enum RespawnInfo
    {
        playerDied,
        manualRespawn
        //levelCompleted
    }
}
