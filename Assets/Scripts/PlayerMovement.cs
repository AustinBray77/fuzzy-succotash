using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Note: Use proper conventions: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

public class PlayerMovement : MonoBehaviour
{

    //Desmos for acceleration: https://www.desmos.com/calculator/emgmts7fzm 

    

    private readonly struct MovementValues
    {
        public MovementValues(float maxAccel, float accelFalloff, float minAccel)
        {
            this.maxAccel = maxAccel;
            this.accelFalloff = accelFalloff;
            this.minAccel = minAccel;
        }

        float maxAccel { get;}
        float accelFalloff { get;}
        float minAccel { get; }
    }

    //In order of priority (if touching multiple, a higher number will take precendence)
    public enum Surface
    {
        air,
        ground
    }

    private readonly Dictionary<Surface, float> speeds = new Dictionary<Surface, float>()
    {
        {Surface.air, 10},
        {Surface.ground, 30}
    };

    private readonly Dictionary<string, Surface> tagToSurface = new Dictionary<string, Surface>()
    {
        {"Ground", Surface.ground}
    };

    private readonly float jumpForce = 2f;

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
        if (ControlsManager.Instance.Jump && highestPrioritySurface == Surface.ground)
        {
            playerRB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        highestPrioritySurface = Surface.air;
    }


    void OnCollisionEnter2D(Collision2D col)
    {
        UpdateCollisionInfo(col);
    }

    void OnCollisionStay2D(Collision2D col)
    {
        UpdateCollisionInfo(col);
    }

    private void UpdateCollisionInfo(Collision2D col)
    {
        string tag = col.gameObject.tag;
        
        //If the tag of the object is a known surface
        //Maybe in the future use a script on the object and don't read from the tag?
        if(tagToSurface.ContainsKey(tag))
        {
            Surface surface = tagToSurface[tag];

            //If the surface you are touching is higher priority 
            if((int)highestPrioritySurface < (int)surface)
            {
                highestPrioritySurface = surface;
            }
        }
    }
}
