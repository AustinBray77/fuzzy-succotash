using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Note: Use proper conventions: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

public class PlayerMovement : Singleton<PlayerMovement>
{
    //In order of priority (if touching multiple, a higher number will take precendence)
    public enum Surface
    {
        air,
        ground
    }

    private readonly Dictionary<Surface, float> speeds = new Dictionary<Surface, float>() {
        {Surface.air, 10},
        {Surface.ground, 30}
        };

    private readonly float jumpForce = 10f;

    private Surface highestPrioritySurface = Surface.air;

    private Rigidbody2D playerRB;

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        Time.fixedDeltaTime = (float)1 / 100; //100 fps
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        float XInput = ControlsManager.Instance.XInput;
        //Debug.Log(XInput);
        if (XInput != 0)
        {
            playerRB.AddForce(new Vector2(XInput * speeds[highestPrioritySurface] * Time.fixedDeltaTime, 0), ForceMode2D.Impulse);
        }

        //Debug.Log(ControlsManager.Instance.Jump);

        //Add condition for jumping (& cool down maybe)
        if (ControlsManager.Instance.Jump)
        {
            playerRB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
