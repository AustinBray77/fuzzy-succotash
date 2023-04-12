using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelController : MonoBehaviour
{
    #region variables
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
    [SerializeField] private LightMapController _lightMapController;

    private const float waitTimeBeforeStart = 0.3f;

    private int currentStage = 0;

    private double levelStartTime;
    private double lastCompletionTime;

    public double LevelStartTime { get => levelStartTime; }
    public double LevelCompletionTime { get => lastCompletionTime; }

    bool CallbacksAdded = false;

    #endregion

    #region Callbacks
    private void RespawnPressedCallback(InputAction.CallbackContext context)
    {
        Respawn(RespawnInfo.manualRespawn);
    }

    private void PausePressedCallback(InputAction.CallbackContext context)
    {
        Debug.Log("Pause Pressed");
        Pause();
    }

    private void AddCallbacks()
    {
        if (CallbacksAdded) return;
        ControlsManager.Instance.AddCallback(ControlsManager.Actions.respawn, RespawnPressedCallback);
        ControlsManager.Instance.AddCallback(ControlsManager.Actions.pause, PausePressedCallback);
        CallbacksAdded = true;
    }

    private void RemoveCallbacks()
    {
        if (!CallbacksAdded) return;
        ControlsManager.Instance.RemoveCallback(ControlsManager.Actions.respawn, RespawnPressedCallback);
        ControlsManager.Instance.RemoveCallback(ControlsManager.Actions.pause, PausePressedCallback);
        CallbacksAdded = false;
    }
    #endregion

    #region Pause Functions
    public void UnpauseFromPauseMenu()
    {
        Unpause();
    }

    private void Unpause()
    {
        //Need to wait for the menu transition to finish before unpausing
        Time.timeScale = 1;
        ControlsManager.Instance.SetInputMaps(ControlsManager.InputMap.gameplay, ControlsManager.InputMap.pause, ControlsManager.InputMap.respawn);
        levelPaused = false;
        AddCallbacks();
        Debug.Log("Unpausing");
    }

    private void Pause()
    {
        Time.timeScale = 0;
        ControlsManager.Instance.SetInputMaps(ControlsManager.InputMap.menus, ControlsManager.InputMap.pause, ControlsManager.InputMap.respawn);
        levelPaused = true;
        RemoveCallbacks();
        StartCoroutine(MenuController.Instance.OpenScreen("PauseMenu", false, false, ControlsManager.InputMap.menus, ControlsManager.InputMap.pause, ControlsManager.InputMap.respawn));
        Debug.Log("Pausing");
    }
    #endregion

    private void Update()
    {
        if (Player.Instance.PlayerPos.y < deathHeight && !Player.Instance.Data.RespawningState)
        {
            Respawn(RespawnInfo.playerDied);
        }
    }

    #region Initialization functions

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

        if (_lightMapController != null)
        {
            _lightMapController.OnSpawn();
        }
    }

    #endregion

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
        AddCallbacks();
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
        ControlsManager.Instance.SetInputMaps(ControlsManager.InputMap.gameplay, ControlsManager.InputMap.pause, ControlsManager.InputMap.respawn);

        levelStartTime = Time.timeAsDouble;
    }

    private void ResetLevel()
    {
        levelChanges.LoadStage(currentStage);
    }

    public void LevelCompleted()
    {
        Player.Instance.Data.RespawningState = true;

        //Time.timeScale = 0;
        lastCompletionTime = Time.timeAsDouble - levelStartTime;

        _data.LogLevelCompletion(currentStage, lastCompletionTime);

        Debug.Log("Stage completed: " + currentStage);
        Debug.Log("Number of Stages: " + _numberOfStages);

        //If the last stage was completed
        if (currentStage >= _numberOfStages - 1)
        {
            ControlsManager.Instance.SetInputMaps(ControlsManager.InputMap.pause);
            RemoveCallbacks();
            StartCoroutine(MenuController.Instance.OpenScreen("EndLevelScreen", false, false, ControlsManager.InputMap.menus, ControlsManager.InputMap.respawn));
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
        RemoveCallbacks();
    }

    public void OnDestroy()
    {
        if (_lightMapController != null)
        {
            Destroy(_lightMapController.gameObject);
        }
    }

    public StageState GetStageState(int stage)
    {
        if (stage < 0 || stage >= _numberOfStages)
        {
            return StageState.Locked;
        }

        if (_data.Completions[stage] > 0)
        {
            return StageState.Completed;
        }

        if (stage == 0)
        {
            return StageState.Unlocked;
        }

        if (_data.Completions[stage - 1] > 0)
        {
            return StageState.Unlocked;
        }

        return StageState.Locked;
    }

    public enum RespawnInfo
    {
        playerDied,
        manualRespawn
        //levelCompleted
    }
}
