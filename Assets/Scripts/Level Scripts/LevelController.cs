using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public bool LevelRunning { get; private set; } = false;
    [SerializeField] Transform playerStartPos;
    //[SerializeField] GameObject levelContainer; //Is this needed?

    private LevelData data;

    [SerializeField] private int _id;
    [SerializeField] private int _numberOfStages;
    [SerializeField] private LevelProgresser levelChanges;

    int currentStage = 0;

    private double levelStartTime;

    private void Start()
    { 
        //
        levelChanges.Initialize();

        //Check if data for this level exists

        //Otherwise 
        data = new LevelData("level_" + _id, _numberOfStages);
        SaveHandler.Instance.AddSaveableComponent(data);
    }

    // Starts the level
    public void StartLevel(int stage = 0)
    {
        levelChanges.LoadStage(stage);

        SpawnPlayer();
    }

    // Respawns the player
    public void Respawn(RespawnInfo info)
    {
        data.LogRespawn(info);

        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        ResetLevel();

        Player.Instance.SetTransform(playerStartPos);

        StartRun();
    }

    private void StartRun()
    {
        data.LogAttemptStart();

        //Enable gameplay inputmap
        ControlsManager.Instance.SetInputMap(ControlsManager.InputMap.gameplay);

        levelStartTime = Time.timeAsDouble;
        Time.timeScale = 1;

        //Tell the timer UI to start

    }

    private void ResetLevel()
    {

    }

    public void PauseLevel()
    {
        Time.timeScale = 0;
        ControlsManager.Instance.SetInputMap(ControlsManager.InputMap.menus);
        //Open pause menu?
    }

    public void LevelCompleted()
    {
        //Pause level and bring up level completion menu?
        Time.timeScale = 0;
        ControlsManager.Instance.SetInputMap(ControlsManager.InputMap.menus);

        double time = Time.timeAsDouble - levelStartTime;

        data.LogLevelCompletion(currentStage, time);
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
