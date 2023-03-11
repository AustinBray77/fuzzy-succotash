using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public bool LevelRunning {get; private set;} = false;
    [SerializeField] Transform playerStartPos;
    [SerializeField] GameObject levelContainer;

    private LevelData data;


    private double levelStartTime;

    private void Start()
    {
        //Check if data for this level exists

        //Otherwise 
        data = new LevelData();
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
        //ControlsManager.

        //...

    }

    private void ResetLevel()
    {

    }





    private void OnDestroy()
    {

    }
}
