using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsManager : Singleton<ControlsManager>
{

    [SerializeField] InputActionAsset inputActions;

    private InputAction xMovementAction;
    private InputAction jumpAction;

    public float XMove;
    public bool jump;

    // Start is called before the first frame update
    void Start()
    {
        xMovementAction = inputActions.FindActionMap("Gameplay").FindAction("X Movement");
        jumpAction = inputActions.FindActionMap("Gameplay").FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        XMove = xMovementAction.ReadValue<float>();
        jump = jumpAction.ReadValue<bool>();
    }
}
