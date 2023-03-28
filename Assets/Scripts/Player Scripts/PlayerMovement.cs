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

    private static MovementValues groundVals = new MovementValues(25, 8, 12, 16, 15f, 0.01f, 3, 0.5f, true);
    private static MovementValues airVals = new MovementValues(7, 5, 8, 6, 6, 0.05f, 1, 0.5f, false);

    //Movement Physics Constants
    private readonly Dictionary<Surface, MovementValues> surfaceProperties = new()
    {
        {Surface.air,           airVals   },
        {Surface.ground,        groundVals},
        {Surface.chargedGround, groundVals},
        {Surface.bouncer,       groundVals},
        {Surface.blower,        groundVals}
    };

    //Jumping
    private readonly float jumpForce = 10f; //change to const later? (or maybe the value will be modifiable?)
    private readonly Vector2 jumpBias = new Vector2(0, 0.03f);
    private bool stillTouchingJumpSurface = false;
    private double jumpTime = 0;
    private bool heldJump = false;
    private Vector2 jumpDirection = Vector2.zero;
    private List<Tuple<Collider2D, Vector2>> savedChargedForces = new();

    //Should these be specific to surfaces?
    private double minExtraJumpTime = 0.1; //Time after jump when extra jump force starts applying 
    private double maxExtraJumpTime = 0.4; //Time after jump when extra jump force stops applying
    private float extraJumpForce = 20f; //per second

    private Surface highestPrioritySurface = Surface.air;
    private Rigidbody2D playerRB;
    private Vector2 totalContactNormals = Vector3.zero;

    private float gravityScale = 2f;

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
    
        //ACCLERATION
        float XInput = ControlsManager.Instance.XInput;
        float accel = 0;
        if (XInput != 0)
        {
            float inputDirection = Mathf.Sign(XInput);
            //Pass in velocity relative to the direction of input                      
            accel = CalculateAcceleration(playerRB.velocity.x * inputDirection, surfaceProperties[highestPrioritySurface]);
            playerRB.AddForce(new Vector2(XInput * accel * Time.fixedDeltaTime, 0), ForceMode2D.Impulse);
        }

        //DECELERATION
        
        //Update decel function in the future to return negative values if given a negative speed?
        float decelX = CalculateDeceleration(Mathf.Abs(playerRB.velocity.x), surfaceProperties[highestPrioritySurface], false);
        float decelY = CalculateDeceleration(Mathf.Abs(playerRB.velocity.y), surfaceProperties[highestPrioritySurface], true);

        //If over current velocity, make it equal to current velocity, otherwise make it the same value, but in the opposite direction of the velocity
        decelX = (decelX >= Mathf.Abs(playerRB.velocity.x)) ? (playerRB.velocity.x * -1) : decelX * -1 * Mathf.Sign(playerRB.velocity.x);
        decelY = (decelY >= Mathf.Abs(playerRB.velocity.y)) ? (playerRB.velocity.y * -1) : decelY * -1 * Mathf.Sign(playerRB.velocity.y);

        //Applies the deceleration force
        playerRB.AddForce(new Vector2(decelX, decelY) * Time.fixedDeltaTime, ForceMode2D.Impulse);
        
        /*
        Vector2 decelDirection = playerRB.velocity.normalized * -1;

        float decel = CalculateDeceleration(playerRB.velocity.magnitude, surfaceProperties[highestPrioritySurface]);

        //if the deceleration is greater than velocity, then add a force which will bring velocity to zero
        if (decel * Time.fixedDeltaTime >= playerRB.velocity.magnitude)
        {
            playerRB.AddForce(-1 * playerRB.velocity, ForceMode2D.Impulse);
        }
        else
        {
            //Applies the deceleration force in the opposite direction of the players velocity
            playerRB.AddForce(decel * Time.fixedDeltaTime * -1 * playerRB.velocity.normalized, ForceMode2D.Impulse);
        }
        */

        //Debug.Log("Accel: " + accel + " Decel: " + decel + " Net: " + (accel * Mathf.Sign(XInput) - decel * playerVelocityDirection) + " Velocity: " + playerRB.velocity.magnitude);
        //Debug.Log(ControlsManager.Instance.Jump);

        //JUMPING
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
            else if (surfaceProperties[highestPrioritySurface].canJump && !stillTouchingJumpSurface)
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
        public readonly float maxAccel, maxAccelEnd, minAccelStart, minAccel;

        public readonly float maxDecel, decelFalloff, minDecel, verticalDecelMultiplier;

        public readonly bool canJump;

        public MovementValues(float maxAccel, float maxAccelEnd, float minAccelStart, float minAccel, float maxDecel, float decelFalloff, float minDecel, float verticalDecelMultiplier, bool canJump)
        {
            this.maxAccel = maxAccel;
            this.maxAccelEnd = maxAccelEnd;
            this.minAccelStart = minAccelStart;
            this.minAccel = minAccel;

            this.maxDecel = maxDecel;
            this.decelFalloff = decelFalloff;
            this.minDecel = minDecel;
            this.verticalDecelMultiplier = verticalDecelMultiplier;

            this.canJump = canJump;
        }

        public MovementValues(MovementValues baseValues, float? maxAccel, float? maxAccelEnd = null, float? minAccelStart = null, float? minAccel = null, float? maxDecel = null, float? decelFalloff = null, float? minDecel = null, float? verticalDecelMultiplier = null, bool? canJump = null)
        {
            this.maxAccel = maxAccel ?? baseValues.maxAccel;
            this.maxAccelEnd = maxAccelEnd ?? baseValues.maxAccelEnd;
            this.minAccelStart = minAccelStart ?? baseValues.minAccelStart;
            this.minAccel = minAccel ?? baseValues.minAccel;

            this.maxDecel = maxDecel ?? baseValues.maxDecel;
            this.decelFalloff = decelFalloff ?? baseValues.decelFalloff;
            this.minDecel = minDecel ?? baseValues.minDecel;
            this.verticalDecelMultiplier = verticalDecelMultiplier ?? baseValues.verticalDecelMultiplier;

            this.canJump = canJump ?? baseValues.canJump;
        }
    }

    /*
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
    */

    float CalculateAcceleration(float velocity, MovementValues values)
    {
        //Desmos for acceleration: https://www.desmos.com/calculator/gcuhlaobxk
        float accel;

        if (velocity <= values.maxAccelEnd)
        {
            accel = values.maxAccel;
        }
        else if (velocity < values.minAccelStart)
        {
            float maxA = values.maxAccel;
            float minA = values.minAccel;
            float x1 = values.maxAccelEnd;
            float x2 = values.minAccelStart;

            accel = ( (maxA - minA) / 2 * Mathf.Cos(Mathf.PI * (velocity - x1) / (x2 - x1)) ) + ((maxA + minA) / 2);
        }
        else
        {
            accel = values.minAccel;
        }

        return accel;
    }

    float CalculateDeceleration(float velocity, MovementValues values, bool yDecel)
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

        return (yDecel) ? (decel * values.verticalDecelMultiplier) : (decel);

    }
}
