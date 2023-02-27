using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Note: Use proper conventions: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

public class ControlsManager : Singleton<ControlsManager>
{

    [SerializeField] private InputActionAsset inputActions;

    private InputAction xMovementAction;
    private InputAction jumpAction;

    public float XInput { get => xMovementAction.ReadValue<float>(); }
    public bool Jump { get => jumpAction.ReadValue<float>() > 0; } 

    // Start is called before the first frame update
    void Start()
    {
        inputActions.FindActionMap("Gameplay").Enable();
        xMovementAction = inputActions.FindActionMap("Gameplay").FindAction("X Movement");
        jumpAction = inputActions.FindActionMap("Gameplay").FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
    }
}
