using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerData))]
public class Player : Singleton<Player>
{
    public static string PlayerTag = "Player";

    //Movement Controller
    private PlayerMovement movement;

    //Data Controller
    private PlayerData data;

    //Public Controller References
    public PlayerMovement Movement { get => movement; }
    public PlayerData Data { get => data; }

    //Allows other scripts to access the player's position
    public Vector3 PlayerPos { get => transform.position; }

    //Function called on start
    protected override void Awake()
    {
        //Singleton set up code
        base.Awake();

        //Gets references
        movement = GetComponent<PlayerMovement>();
        data = GetComponent<PlayerData>();
        tag = PlayerTag;
        SetActive(false);
    }

    public void Initialize()
    {
        movement.Initialize();
    }

    public void SetTransform(Transform newTransform)
    {
        //Double check that this is supposed to be world position/rotation and not local
        transform.SetPositionAndRotation(newTransform.position, newTransform.rotation);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}