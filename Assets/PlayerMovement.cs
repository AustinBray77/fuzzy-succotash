using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    [SerializeField] inputActionAsset inputActions;

    private InputAction XInput;
    private InputAction Jump;

public class PlayerMovement : Singleton<PlayerMovement>
{
    public enum SurfaceType
    {
        air,
        ground
    }

    void Awake()
    {
        XInput = inputActions.FindActionMap("Gameplay").FIND
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {

    }
}
