using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerData))]
public class Player : Singleton<Player>
{
    //Movement Controller
    private PlayerMovement movement;

    //Data Controller
    private PlayerData data;

    //Public Controller References
    public PlayerMovement Movement { get => movement; }
    public PlayerData Data { get => data; }

    //Function called on start
    private void Start()
    {
        //Gets references
        movement = GetComponent<PlayerMovement>();
        data = GetComponent<PlayerData>();
    }
}
