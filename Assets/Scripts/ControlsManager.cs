using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Note: Use proper conventions: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

public class ControlsManager : Singleton<ControlsManager>
{
    public enum InputMap
    {
        gameplay,
        menus
    }

    private Dictionary<InputMap, string> mapNames = new()
    {
        {InputMap.gameplay, "GamePlay" },
        {InputMap.menus, "Menus" }
    };

    [SerializeField] private InputActionAsset inputActions;

    private InputAction xMovementAction;
    private InputAction jumpAction;

    public float XInput { get => xMovementAction.ReadValue<float>(); }
    public bool Jump { get => jumpAction.ReadValue<float>() > 0; } 

    // Start is called before the first frame update
    void Start()
    {
        inputActions.FindActionMap(mapNames[InputMap.gameplay]).Enable();
        xMovementAction = inputActions.FindActionMap(mapNames[InputMap.gameplay]).FindAction("X Movement");
        jumpAction = inputActions.FindActionMap(mapNames[InputMap.gameplay]).FindAction("Jump");
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

    // Update is called once per frame
    void Update()
    {
    }
}
