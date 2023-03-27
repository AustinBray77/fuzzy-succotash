using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelController : MonoBehaviour
{
    //public bool LevelRunning { get; private set; } = false;
    private bool levelPaused = false;
    [SerializeField] Transform playerStartPos;

    private LevelData _data;
    public LevelData Data { get => _data; }

    [SerializeField] private int _id;
    private int _numberOfStages;
    [SerializeField] private string _title;
    [SerializeField] private LevelProgresser levelChanges;
    [SerializeField] private float deathHeight;

    private const float waitTimeBeforeStart = 0.25f;

    private int currentStage = 0;

    private double levelStartTime;
    private double lastCompletionTime;

    public double LevelCompletionTime { get => lastCompletionTime; }

    private void OnEnable()
    {
        //Adds callbacks for the respawn and pause buttons
        ControlsManager.Instance.AddCallBack(ControlsManager.Actions.respawn, RespawnPressedCallback);
        ControlsManager.Instance.AddCallBack(ControlsManager.Actions.pause, PausePressedCallback);
    }

    private void RespawnPressedCallback(InputAction.CallbackContext context)
    {
        Respawn(RespawnInfo.manualRespawn);
    }

    private void PausePressedCallback(InputAction.CallbackContext context)
    {
        Debug.Log("Pause Pressed");
        PausePressed();
    }

    private void Update()
    {
        if (Player.Instance.PlayerPos.y < deathHeight && !Player.Instance.Data.RespawningState)
        {
            Respawn(RespawnInfo.playerDied);
        }
    }

    //Called when the game loads, called once per run
    public void Initialize(int index)
    {
        playerStartPos.gameObject.SetActive(false);

        //Reinitialize the data
        _numberOfStages = levelChanges.GetNumberOfStagesPreInit();
        _data = new LevelData("level_" + _id, _numberOfStages, _title, index);
        SaveHandler.Instance.AddSaveableComponent(_data);
    }

    public void OnSpawn()
    {
        _data = (LevelData)SaveHandler.Instance.GetSaveableComponent("level_" + _id);

        //Set up the level Progressor so that it can easily move through stages later on
        levelChanges.Initialize();
        _numberOfStages = levelChanges.NumberOfStages;
    }

    public void RestartLevelFromMenu()
    {
        Unpause();
        StartLevel(0);
    }

    // Starts the level
    public void StartLevel(int stage, float extraWaitTime = 0)
    {
        ControlsManager.Instance.SetInputMaps(ControlsManager.InputMap.pause);
        currentStage = stage;

        StartCoroutine(SpawnPlayer(extraWaitTime));
    }

    public void RespawnFromMenu()
    {
        Unpause();
        Respawn(RespawnInfo.manualRespawn);
    }

    // Respawns the player
    private void Respawn(RespawnInfo info)
    {
        ControlsManager.Instance.SetInputMaps(ControlsManager.InputMap.pause);
        //Debug.Log(_data);
        _data.LogRespawn(currentStage, info);

        StartCoroutine(SpawnPlayer());
    }

    private IEnumerator SpawnPlayer(float extraWaitTime = 0)
    {
        Player.Instance.Data.RespawningState = true;
        Player.Instance.Spawn(playerStartPos);
        Time.timeScale = 1;
        ResetLevel();

        yield return new WaitForSeconds(waitTimeBeforeStart + extraWaitTime); //Maybe change to realtime if timescale is 0
        yield return new WaitForFixedUpdate(); //If you do not wait for a fixed update, there is variation in the start time relative to the next fixed update, adding randomness to the finish time

        StartRun();
        
        //StartRun();
        //Fade out
        /*if (!AnimationManager.Instance.FadeToColour(Color.black, SpawnPlayerPart2))
        {
            Debug.LogWarning("Could not start fade out");
        }*/
    }

    /*private void SpawnPlayerPart2()
    {
        Player.Instance.Spawn(playerStartPos);

        ResetLevel();

        //Fade in (and start the run once the fade is finished)
        if (!AnimationManager.Instance.FadeFromColour(StartRun))
        {
            Debug.LogWarning("Could not start fade in");
        }
    }*/

    private void StartRun()
    {
        //Debug.Log(_data.Attempts[0]);
        //Debug.Log(currentStage);
        _data.LogAttemptStart(currentStage);

        //Turn off invincibility
        Player.Instance.Data.RespawningState = false;

        //Enable gameplay inputmap
        ControlsManager.Instance.SetInputMaps(ControlsManager.InputMap.gameplay, ControlsManager.InputMap.pause);

        levelStartTime = Time.timeAsDouble;
        
        //This should be done earlier, otherwise countdown will never finish
        Time.timeScale = 1;

        //Tell the timer UI to start
    }

    private void ResetLevel()
    {
        levelChanges.LoadStage(currentStage);
    }

    public void UnpauseFromPauseMenu()
    {
        Unpause();
    }

    private void PausePressed()
    {
        if (levelPaused)
        {
            Unpause();
        }
        else
        {
            Pause();
        }
    }

    private void Unpause()
    {
        //Remove this once inputs are added to menus
        StartCoroutine(MenuController.Instance.OpenScreen("Game", false, false));

        Time.timeScale = 1;
        ControlsManager.Instance.SetInputMaps(ControlsManager.InputMap.gameplay, ControlsManager.InputMap.pause);
        levelPaused = false;
    }

    private void Pause()
    {
        Time.timeScale = 0;
        ControlsManager.Instance.SetInputMaps(ControlsManager.InputMap.menus, ControlsManager.InputMap.pause);
        levelPaused = true;

        StartCoroutine(MenuController.Instance.OpenScreen("PauseMenu", false, false));
    }

    public void LevelCompleted()
    {
        Player.Instance.Data.RespawningState = true;

        //Time.timeScale = 0;
        ControlsManager.Instance.SetInputMaps(ControlsManager.InputMap.pause);

        lastCompletionTime = Time.timeAsDouble - levelStartTime;

        _data.LogLevelCompletion(currentStage, lastCompletionTime);

        Debug.Log("Stage completed: " + currentStage);
        Debug.Log("Number of Stages: " + _numberOfStages);

        //If the last stage was completed
        if (currentStage >= _numberOfStages - 1)
        {
            StartCoroutine(MenuController.Instance.OpenScreen("EndLevelScreen", false, false));
        }
        else
        {
            NextStage();
        }

    }

    private void NextStage()
    {
        //Add nice animations to show things being activated

        //Temp code for now
        Debug.Log("Starting next Stage");
        currentStage++;
        ResetLevel();
        levelChanges.NextStage(currentStage);
        StartCoroutine(SpawnPlayer());
    }

    private void OnDisable()
    {
        //Removes the callbacks from the actions
        ControlsManager.Instance.RemoveCallBack(ControlsManager.Actions.respawn, RespawnPressedCallback);
        ControlsManager.Instance.RemoveCallBack(ControlsManager.Actions.pause, PausePressedCallback);
    }

    public enum RespawnInfo
    {
        playerDied,
        manualRespawn
        //levelCompleted
    }
}
