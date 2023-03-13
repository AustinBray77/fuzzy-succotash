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
    public void StartLevel()
    {
        SpawnPlayer();
    }

    // Respawns the level
    public void Respawn()
    {
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


    private void OnDestroy()
    {

    }
}
