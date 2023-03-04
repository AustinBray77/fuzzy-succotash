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

    private readonly Dictionary<string, Surface> tagToSurface = new Dictionary<string, Surface>()
    {
        {"Ground", Surface.ground}
    };

    //Movement Physics Constants
    private readonly Dictionary<Surface, MovementValues> surfaceProperties = new()
    {
        //{Surface.air, new MovementValues (5, 0.005f, 0.5f)},
        //{Surface.ground, new MovementValues (20, 0.01f, 1)}
        {Surface.air, new MovementValues (10, 0, 10, 10, 0.01f, 2)},
        {Surface.ground, new MovementValues (25, 0, 25, 25, 0.01f, 5)}
    };

    //Jumping
    private readonly float jumpForce = 10f;
    private bool stillTouchingJumpSurface = false;


    private Surface highestPrioritySurface = Surface.air;
    private Rigidbody2D playerRB;
    private Vector2 totalContactNormals = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        Time.fixedDeltaTime = (float)1 / 100; //100 fps
    }

    void FixedUpdate()
    {
        //Acceleration
        float XInput = ControlsManager.Instance.XInput;
        //Debug.Log(XInput);
        float accel = 0;
        if (XInput != 0)
        {
            float inputDirection = Mathf.Sign(XInput);
                                              //Velocity relative to the direction of input                      
            accel = CalculateAcceleration(playerRB.velocity.x * inputDirection, surfaceProperties[highestPrioritySurface]);
            playerRB.AddForce(new Vector2(XInput * accel * Time.fixedDeltaTime, 0), ForceMode2D.Impulse);
            
        }

        //Decleration
        float playerVelocityDirection = Mathf.Sign(playerRB.velocity.x);
        float decel = CalculateDeceleration(Mathf.Abs(playerRB.velocity.x), surfaceProperties[highestPrioritySurface]);
        playerRB.AddForce(new Vector2(playerVelocityDirection * -1 * decel * Time.fixedDeltaTime, 0), ForceMode2D.Impulse);

        Debug.Log("Accel: " + accel + " Decel: " + decel + " Net: " + (accel * Mathf.Sign(XInput) - decel * playerVelocityDirection));

        //Debug.Log(ControlsManager.Instance.Jump);

        //Add condition for jumping
        if (highestPrioritySurface == Surface.air)
        {
            stillTouchingJumpSurface = false;
        }
        if (ControlsManager.Instance.Jump && highestPrioritySurface == Surface.ground && !stillTouchingJumpSurface)
        {
            playerRB.AddForce(totalContactNormals.normalized * jumpForce, ForceMode2D.Impulse);
            stillTouchingJumpSurface = true;
            Debug.Log("Jump");
        }

        highestPrioritySurface = Surface.air;
        totalContactNormals = Vector3.zero;
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

        ContactPoint2D[] contacts = new ContactPoint2D[col.contactCount];
        col.GetContacts(contacts);
        foreach(ContactPoint2D point in contacts)
        {
            totalContactNormals += point.normal;
        }
        
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

    private readonly struct MovementValues
    {
        public MovementValues(float maxAccel, float accelFalloff, float minAccel, float maxDecel, float decelFalloff, float minDecel)
        {
            this.maxAccel = maxAccel;
            this.accelFalloff = accelFalloff;
            this.minAccel = minAccel;

            this.maxDecel = maxDecel;
            this.decelFalloff = decelFalloff;
            this.minDecel = minDecel;
        }

        public readonly float maxAccel;
        public readonly float accelFalloff;
        public readonly float minAccel;

        public readonly float maxDecel;
        public readonly float decelFalloff;
        public readonly float minDecel;
    }

    float CalculateAcceleration(float velocity, MovementValues values)
    {
        //Desmos for acceleration: https://www.desmos.com/calculator/emgmts7fzm
        float accel;
        if (velocity <= 0)
        {
            accel = values.maxAccel;
        }
        else if(values.maxAccel - values.minAccel == 0)
        {
            accel = values.maxAccel;
        }
        else
        {
            accel = (1 / ((values.accelFalloff * velocity) + (1 / (values.maxAccel - values.minAccel)))) + values.minAccel;
        }

        return accel;
    }

    float CalculateDeceleration(float velocity, MovementValues values)
    {
        //Desmos for Decel: https://www.desmos.com/calculator/yd6t9wniem

        float decel;
        if (velocity <= 0)
        {
            decel = 0;
        }
        else if (values.maxDecel - values.minDecel == 0)
        {
            decel = values.maxDecel;
        }
        else
        {
            decel = (-1 / ((values.decelFalloff * velocity) + (1 / (values.maxDecel - values.minDecel)))) + values.maxDecel;
        }

        return decel;

    }
}
