using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

//Note: Use proper conventions: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

public class ControlsManager : Singleton<ControlsManager>
{
    [SerializeField] private InputActionAsset inputActions;

    public enum InputMap
    {
        gameplay,
        menus,
        pause
    }

    private readonly Dictionary<InputMap, string> mapNames = new()
    {
        {InputMap.gameplay, "GamePlay" },
        {InputMap.menus, "Menus" },
        {InputMap.pause, "PauseMenu" }
    };

    //All actions regardless of InputMap
    public enum Actions
    {
        xMovement,
        jump,
        respawn,
        pause
    }

    private Dictionary<Actions, InputAction> enumToAction;

    //Gameplay actions
    private InputAction xMovementAction;
    private InputAction jumpAction;
    private InputAction respawnAction;

        //Values of actions
        public float XInput { get => xMovementAction.ReadValue<float>(); }
        public bool Jump { get => jumpAction.IsPressed(); }
        //add respawn if needed

    //Pause
    private InputAction pauseAction;
    
        //Add pause value return if needed

    //Menus


    public void Initialize()
    {
        //Gameplay
        xMovementAction = inputActions.FindActionMap(mapNames[InputMap.gameplay]).FindAction("X Movement");
        jumpAction = inputActions.FindActionMap(mapNames[InputMap.gameplay]).FindAction("Jump");
        respawnAction = inputActions.FindActionMap(mapNames[InputMap.gameplay]).FindAction("Respawn");

        //Pause
        pauseAction = inputActions.FindActionMap(mapNames[InputMap.pause]).FindAction("Pause");

        enumToAction = new()
        {
            //Gameplay
            { Actions.xMovement, xMovementAction },
            { Actions.jump, jumpAction },
            { Actions.respawn, respawnAction },

            //Pause
            { Actions.pause, pauseAction }
        };
    }

    public void AddCallBack(Actions action, Action<InputAction.CallbackContext> callback)
    {
        //Debug.Log(action);
        //Debug.Log(enumToAction[action]);
        enumToAction[action].performed += callback;
    }

    public void RemoveCallBack(Actions action, Action<InputAction.CallbackContext> callback)
    {
        enumToAction[action].performed -= callback;
    }

    /// <summary>
    /// Enables the input maps given and disables all of the other ones
    /// </summary>
    /// <param name="newMaps"> input maps to enable</param>
    public void SetInputMaps(params InputMap[] newMaps)
    {
        foreach (InputMap map in mapNames.Keys)
        {
            if (Array.IndexOf(newMaps, map) != -1)
            {
                inputActions.FindActionMap(mapNames[map]).Enable();
            }
            else
            {
                inputActions.FindActionMap(mapNames[map]).Disable();
            }
        }
    }

    public void DisableInputMap(InputMap newMap)
    {
        inputActions.FindActionMap(mapNames[newMap]).Disable();
    }

    public void DisableInput()
    {
        //Add code
        foreach (InputMap map in mapNames.Keys)
        {
            inputActions.FindActionMap(mapNames[map]).Disable();
        }
    }
}
