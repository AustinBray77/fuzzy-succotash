using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

//Note: Use proper conventions: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

public class PlayerMovement : MonoBehaviour
{
    //In order of priority (if touching multiple, a higher number will take precendence)
    public enum Surface
    {
        air,
        ground,
        chargedGround,
    }

    private readonly Dictionary<string, Surface> tagToSurface = new Dictionary<string, Surface>()
    {
        {"Ground", Surface.ground},
        {"Charged Ground", Surface.chargedGround}
    };

    //Movement Physics Constants
    private readonly Dictionary<Surface, MovementValues> surfaceProperties = new()
    {
        //{Surface.air, new MovementValues (5, 0.005f, 0.5f)},
        //{Surface.ground, new MovementValues (20, 0.01f, 1)}
        {Surface.air, new MovementValues (10, 0.05f, 7, 7, 0.03f, 1, false)},
        {Surface.ground, new MovementValues (40, 0.01f, 28, 30, 0.01f, 3, true)},
        {Surface.chargedGround, new MovementValues (40, 0.01f, 28, 30, 0.01f, 3, true)}
    };

    //Jumping
    private readonly float jumpForce = 8f; //change to const later? (or maybe the value will be modifiable?)
    private readonly Vector2 jumpBias = new Vector2(0, 0.03f);
    private bool stillTouchingJumpSurface = false;
    private double jumpTime = 0;
    private bool heldJump = false;
    private Vector2 jumpDirection = Vector2.zero;
    private List<Tuple<Collider2D, Vector2>> savedChargedForces = new();

    //Should these be specific to surfaces?
    private double minExtraJumpTime = 0.1; //Time after jump when extra jump force starts applying 
    private double maxExtraJumpTime = 0.5; //Time after jump when extra jump force stops applying
    private float extraJumpForce = 14f; //per second

    private Surface highestPrioritySurface = Surface.air;
    private Rigidbody2D playerRB;
    private Vector2 totalContactNormals = Vector3.zero;

    private Vector2 gravity = new Vector2(0, -14);

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        Time.fixedDeltaTime = (float)1 / 100; //100 fps
        savedChargedForces.Clear();
        Physics2D.gravity = gravity;
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
        if(decel * Time.fixedDeltaTime >= Mathf.Abs(playerRB.velocity.x))
        {
            playerRB.AddForce(new Vector2(-1 * playerRB.velocity.x, 0), ForceMode2D.Impulse);
        }
        else
        {
            playerRB.AddForce(new Vector2(playerVelocityDirection * -1 * decel * Time.fixedDeltaTime, 0), ForceMode2D.Impulse);
        }

        //Debug.Log("Accel: " + accel + " Decel: " + decel + " Net: " + (accel * Mathf.Sign(XInput) - decel * playerVelocityDirection));

        //Debug.Log(ControlsManager.Instance.Jump);

    //Jumping
        if (!ControlsManager.Instance.Jump)
        {

            heldJump = false;
        }
        if (highestPrioritySurface == Surface.air)
        {
            stillTouchingJumpSurface = false;
        }
         
        if(ControlsManager.Instance.Jump)
        {
            //This order of conditions means that if you have held the spacebar since you won't jump again until you release and press it again
            //This can be changed to auto jump after the max force has been given (by setting heldJump to false after the time is up)
            //Or it can be completely removed (by changing the else if to and if)
            if(heldJump)
            {
                double timeAfterJump = Time.timeAsDouble - jumpTime;
                if (timeAfterJump >= minExtraJumpTime && timeAfterJump <= maxExtraJumpTime)
                {
                    playerRB.AddForce(extraJumpForce * Time.fixedDeltaTime * jumpDirection, ForceMode2D.Impulse);
                }
            }
            if (surfaceProperties[highestPrioritySurface].canJump && !stillTouchingJumpSurface)
            {
                jumpDirection = (totalContactNormals.normalized + jumpBias).normalized;
                Vector2 force = jumpDirection * jumpForce;

                if (savedChargedForces.Count > 0)
                {
                    foreach (Tuple<Collider2D, Vector2> item in savedChargedForces)
                    {
                        force += item.Item2;
                    }
                }

                savedChargedForces.Clear();
                playerRB.AddForce(force, ForceMode2D.Impulse);
                
                stillTouchingJumpSurface = true;
                heldJump = true;
                jumpTime = Time.timeAsDouble;
                //Debug.Log("Jump");
            }
        }


        highestPrioritySurface = Surface.air;
        totalContactNormals = Vector3.zero;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log(col.gameObject.name);
        //Debug.Log(col.collider.gameObject.name);

        UpdateCollisionInfo(col);

        //Saving charges for charged walls
        if(col.gameObject.CompareTag("Charged Ground"))
        {
            Vector2 avgNormal = Vector2.zero;
            
            ContactPoint2D[] contacts = new ContactPoint2D[col.contactCount];
            col.GetContacts(contacts);
            foreach (ContactPoint2D point in contacts)
            {
                avgNormal += point.normal;
            }
            avgNormal.Normalize();

            Debug.Log(avgNormal);
            Debug.Log(col.relativeVelocity);
            Debug.Log(Vector2.Dot(avgNormal, col.relativeVelocity));

            Vector2 force = avgNormal * Vector2.Dot(avgNormal, col.relativeVelocity);

            savedChargedForces.Add(new Tuple<Collider2D, Vector2>(col.collider, force));
            Debug.Log(col.collider + " " + force);
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        UpdateCollisionInfo(col);
    }

    
    private void OnCollisionExit2D(Collision2D col)
    {
        //Debug.Log(col.gameObject.name);
        //Debug.Log(col.collider.gameObject.name);

        //Remove force charges
        if (col.gameObject.CompareTag("Charged Ground"))
        {
            //Checks if the gameobject matches the colliders gameobject
            bool match(Tuple<Collider2D, Vector2> obj) => (obj.Item1 == col.collider);

            //Removes all forces that are matching with the gameobject to one currently being left
            savedChargedForces.RemoveAll(match);
        }
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
        public MovementValues(float maxAccel, float accelFalloff, float minAccel, float maxDecel, float decelFalloff, float minDecel, bool canJump)
        {
            this.maxAccel = maxAccel;
            this.accelFalloff = accelFalloff;
            this.minAccel = minAccel;

            this.maxDecel = maxDecel;
            this.decelFalloff = decelFalloff;
            this.minDecel = minDecel;

            this.canJump = canJump;
        }

        public readonly float maxAccel;
        public readonly float accelFalloff;
        public readonly float minAccel;

        public readonly float maxDecel;
        public readonly float decelFalloff;
        public readonly float minDecel;

        public readonly bool canJump;
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
