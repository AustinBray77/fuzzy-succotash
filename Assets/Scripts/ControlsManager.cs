using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

//Note: Use proper conventions: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

public class ControlsManager : Singleton<ControlsManager>
{
    public enum InputMap
    {
        gameplay,
        menus
    }

    private readonly Dictionary<InputMap, string> mapNames = new()
    {
        {InputMap.gameplay, "GamePlay" },
        {InputMap.menus, "Menus" }
    };

    public enum ActionType
    {
        xMovement,
        jump,
        respawn
    }

    private Dictionary<ActionType, InputAction> actionTypeToAction;

    [SerializeField] private InputActionAsset inputActions;

    private static InputAction xMovementAction;
    private static InputAction jumpAction;
    private static InputAction respawnAction;

    public enum Actions
    {
        xMovement,
        jump,
        respawn
    }

    private Dictionary<Actions, InputAction> enumToAction = new()
    {
        { Actions.xMovement, xMovementAction },
        { Actions.jump, jumpAction },
        { Actions.respawn, respawnAction }
    };

    public float XInput { get => xMovementAction.ReadValue<float>(); }
    public bool Jump { get => jumpAction.IsPressed(); }

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        inputActions.FindActionMap(mapNames[InputMap.gameplay]).Enable();
        xMovementAction = inputActions.FindActionMap(mapNames[InputMap.gameplay]).FindAction("X Movement");
        jumpAction = inputActions.FindActionMap(mapNames[InputMap.gameplay]).FindAction("Jump");
        respawnAction = inputActions.FindActionMap(mapNames[InputMap.gameplay]).FindAction("Respawn");
    }

    public void AddCallBack(Actions action, Action<InputAction.CallbackContext> callback)
    {
        enumToAction[action].performed += callback;
    }

    public void SetInputMap(InputMap newMap)
    {
        //Add code
        foreach (InputMap map in mapNames.Keys)
        {
            if (map == newMap)
            {
                inputActions.FindActionMap(mapNames[map]).Enable();
            }
            else
            {
                inputActions.FindActionMap(mapNames[map]).Disable();
            }
        }
    }

    public void DiableInput()
    {
        //Add code
        foreach (InputMap map in mapNames.Keys)
        {
            inputActions.FindActionMap(mapNames[map]).Disable();
        }
    }
}
