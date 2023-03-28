using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

//Note: Use proper conventions: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    //In order of priority (if touching multiple, a higher number will take precendence)
    public enum Surface
    {
        air,
        chargedGround,
        bouncer,
        blower,
        ground
    }

    public static readonly List<Tuple<string, Surface>> tagToSurface = new List<Tuple<string, Surface>>()
    {
        new Tuple<string, Surface> ("Ground", Surface.ground),
        new Tuple<string, Surface> ("Charged Ground", Surface.chargedGround),
        new Tuple<string, Surface> ("Bouncer", Surface.bouncer),
        new Tuple<string, Surface> ("Blower", Surface.blower)
    };

    public static string TagFromSurface(Surface surface)
    {
        foreach (Tuple<string, Surface> tuple in tagToSurface)
        {
            if (tuple.Item2 == surface)
            { return tuple.Item1; }
        }
        return null;
    }

    public static Surface? SurfaceFromTag(string tag)
    {
        foreach (Tuple<string, Surface> tuple in tagToSurface)
        {
            if (tuple.Item1 == tag)
            { return tuple.Item2; }
        }
        return null;
    }

    private static MovementValues groundVals = new MovementValues(40, 0.01f, 28, 30, 0.01f, 3, true);

    //Movement Physics Constants
    private readonly Dictionary<Surface, MovementValues> surfaceProperties = new()
    {
        //{Surface.air, new MovementValues (5, 0.005f, 0.5f)},
        //{Surface.ground, new MovementValues (20, 0.01f, 1)}
        
        //Update to have default ground movement separate
        //Add secondary new (creation) function for Movement values that allows you to pass in info, and then modify some value
        {Surface.air,           new MovementValues (10, 0.05f,  7,  7, 0.03f, 1, false)},
        {Surface.ground,        groundVals},
        {Surface.chargedGround, groundVals},
        {Surface.bouncer,       groundVals},
        {Surface.blower,        groundVals}
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
    private double maxExtraJumpTime = 0.4; //Time after jump when extra jump force stops applying
    private float extraJumpForce = 17f; //per second

    private Surface highestPrioritySurface = Surface.air;
    private Rigidbody2D playerRB;
    private Vector2 totalContactNormals = Vector3.zero;

    private float gravityScale = 1.5f;

    // Start is called before the first frame update
    public void Initialize()
    {
        playerRB = GetComponent<Rigidbody2D>();
        Time.fixedDeltaTime = (float)1 / 100; //100 fps
        playerRB.gravityScale = gravityScale;

        ResetMovement();
    }

    public void SetKinematic(bool isKinematic)
    {
        playerRB.isKinematic = isKinematic;
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

        //if the deceleration is greater than velocity, then add a force which will bring velocity to zero
        if (decel * Time.fixedDeltaTime >= Mathf.Abs(playerRB.velocity.x))
        {
            playerRB.AddForce(new Vector2(-1 * playerRB.velocity.x, 0), ForceMode2D.Impulse);
        }
        else
        {
            playerRB.AddForce(new Vector2(playerVelocityDirection * -1 * decel * Time.fixedDeltaTime, 0), ForceMode2D.Impulse);
        }

        //Debug.Log("Accel: " + accel + " Decel: " + decel + " Net: " + (accel * Mathf.Sign(XInput) - decel * playerVelocityDirection) + " Velocity: " + playerRB.velocity.magnitude);

        //Debug.Log(ControlsManager.Instance.Jump);

        //Jumping

        if (!ControlsManager.Instance.Jump)
        {
            if(heldJump)
            {
                Debug.Log("Time Jump Key Held: " + (Time.timeAsDouble - jumpTime));
            }

            heldJump = false;
            stillTouchingJumpSurface = false;

        }
        //StillTouchingJumpSurface is a safety to prevent jumping, but not leaving the ground, meaning that you could jump an infinite number of time
        //Also I believe you are still touching the ground for one more frame after jumping, so you would jump twice without it
        if (highestPrioritySurface == Surface.air)
        {
            stillTouchingJumpSurface = false;
        }

        if (ControlsManager.Instance.Jump)
        {
            //This order of conditions means that if you have held the spacebar since you won't jump again until you release and press it again
            //This can be changed to auto jump after the max force has been given (by setting heldJump to false after the time is up)
            //Or it can be completely removed (by changing the else if to and if)

            //If still holding jump & with time limit then add extra force
            if (heldJump)
            {
                double timeAfterJump = Time.timeAsDouble - jumpTime;
                if (timeAfterJump >= minExtraJumpTime && timeAfterJump <= maxExtraJumpTime)
                {
                    playerRB.AddForce(extraJumpForce * Time.fixedDeltaTime * jumpDirection, ForceMode2D.Impulse);
                }
                else if (timeAfterJump > maxExtraJumpTime)
                {
                    stillTouchingJumpSurface = false;
                }
            }

            //If holding jump and able to jump then jump
            /*else*/
            if (surfaceProperties[highestPrioritySurface].canJump && !stillTouchingJumpSurface)
            {
                jumpDirection = (totalContactNormals.normalized + jumpBias).normalized;
                Vector2 force = jumpDirection * jumpForce;

                //Add forces from charged walls into jump
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
        if (col.gameObject.CompareTag("Charged Ground"))
        {
            Vector2 avgNormal = Vector2.zero;

            ContactPoint2D[] contacts = new ContactPoint2D[col.contactCount];
            col.GetContacts(contacts);
            foreach (ContactPoint2D point in contacts)
            {
                avgNormal += point.normal;
            }
            avgNormal.Normalize();

            //Debug.Log(avgNormal);
            //Debug.Log(col.relativeVelocity);
            //Debug.Log(Vector2.Dot(avgNormal, col.relativeVelocity));

            //Gets your velocity in the axis of the wall normal
            Vector2 force = avgNormal * Vector2.Dot(avgNormal, col.relativeVelocity);

            //Adds the force to a list of charges, which are released when you jump (or removed when you stop touching the object)
            savedChargedForces.Add(new Tuple<Collider2D, Vector2>(col.collider, force));
            //Debug.Log(col.collider + " " + force);
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

        //Removes charged ground saved force charges
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

        //adds the average direction of the contact normal (for finding jumping direction) 
        ContactPoint2D[] contacts = new ContactPoint2D[col.contactCount];
        col.GetContacts(contacts);
        foreach (ContactPoint2D point in contacts)
        {
            totalContactNormals += point.normal;
        }

        //If the tag matches a surface type, and that surface is higher in priority than the current highest, then replace the highest priority surface
        Surface? surface = SurfaceFromTag(tag);

        if (surface is Surface s && (int)highestPrioritySurface < (int)s)
        {
            highestPrioritySurface = s;
        }

        foreach (Tuple<string, Surface> pair in tagToSurface)
        {

            if (pair.Item1 == tag)
            {
                if ((int)highestPrioritySurface < (int)pair.Item2)
                {
                    highestPrioritySurface = pair.Item2;
                }
            }
        }
    }

    public void ResetMovement()
    {
        playerRB.velocity = Vector2.zero;
        playerRB.angularVelocity = 0;

        stillTouchingJumpSurface = false;
        heldJump = false;
        jumpDirection = Vector2.zero;
        highestPrioritySurface = Surface.air;
        totalContactNormals = Vector3.zero;
        savedChargedForces.Clear();
    }

    private readonly struct MovementValues
    {
        public readonly float maxAccel;
        public readonly float accelFalloff;
        public readonly float minAccel;

        public readonly float maxDecel;
        public readonly float decelFalloff;
        public readonly float minDecel;

        public readonly bool canJump;

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

        public MovementValues(MovementValues baseValues, float? maxAccel = null, float? accelFalloff = null, float? minAccel = null, float? maxDecel = null, float? decelFalloff = null, float? minDecel = null, bool? canJump = null)
        {
            this.maxAccel = maxAccel ?? baseValues.maxAccel;
            this.accelFalloff = accelFalloff ?? baseValues.accelFalloff;
            this.minAccel = minAccel ?? baseValues.minAccel;

            this.maxDecel = maxDecel ?? baseValues.maxDecel;
            this.decelFalloff = decelFalloff ?? baseValues.decelFalloff;
            this.minDecel = minDecel ?? baseValues.minDecel;

            this.canJump = canJump ?? baseValues.canJump;
        }


    }

    float CalculateAcceleration(float velocity, MovementValues values)
    {
        //Desmos for acceleration: https://www.desmos.com/calculator/emgmts7fzm
        float accel;

        if (velocity <= 0)
        {
            accel = values.maxAccel;
        }
        //avoids a division by zero
        //In this case the function is just a straight line, so either max or min can be taken
        else if (values.maxAccel - values.minAccel == 0)
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
        //avoids a division by zero
        //In this case the function is just a straight line, so either max or min can be taken
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
