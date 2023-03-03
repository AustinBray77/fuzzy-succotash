using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Note: Use proper conventions: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

public class PlayerMovement : MonoBehaviour
{
    //In order of priority (if touching multiple, a higher number will take precendence)
    public enum Surface
    {
        air,
        ground
    }

    private readonly struct MovementValues
    {
        public MovementValues(float maxAccel, float accelFalloff, float minAccel)
        {
            this.maxAccel = maxAccel;
            this.accelFalloff = accelFalloff;
            this.minAccel = minAccel;
        }

        public readonly float maxAccel;
        public readonly float accelFalloff;
        public readonly float minAccel;
    }

    float CalculateAcceleration(float velocity, MovementValues values)
    {
        //Desmos for acceleration: https://www.desmos.com/calculator/emgmts7fzm
        
        if(velocity <= 0)
        {
            return values.maxAccel;
        }
        else
        {
            return (1 / ((values.accelFalloff * velocity) + (1 / (values.maxAccel - values.minAccel)))) + values.minAccel;
        }
    }

    private readonly Dictionary<Surface, MovementValues> surfaceProperties = new Dictionary<Surface, MovementValues>()
    {
        {Surface.air, new MovementValues (5, 0.005f, 0.5f)},
        {Surface.ground, new MovementValues (20, 0.01f, 1)}
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
            float accel = CalculateAcceleration(playerRB.velocity.x * XInput, surfaceProperties[highestPrioritySurface]);
            playerRB.AddForce(new Vector2(XInput * accel * Time.fixedDeltaTime, 0), ForceMode2D.Impulse);
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
