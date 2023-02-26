using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsManager : Singleton<ControlsManager>
{

    [SerializeField] InputActionAsset inputActions;

    private InputAction xMovementAction;
    private InputAction jumpAction;

    public int XMove;
    public bool jump;

    // Start is called before the first frame update
    void Start()
    {
        xMovementAction = actions.FindActionMap("Gameplay").FindAction("X Movement");
        jumpAction = actions.FindActionMap("Gameplay").FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
