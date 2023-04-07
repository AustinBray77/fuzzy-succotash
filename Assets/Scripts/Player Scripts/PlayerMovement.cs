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
    #region Enums and Related Variables & Functions
    //In order of priority (if touching multiple, a higher number will take precendence)
    public enum Surface
    {
        air,
        chargedGround,
        bouncer,
        blower,
        ground,
        booster
    }

    public static readonly List<Tuple<string, Surface>> tagToSurface = new List<Tuple<string, Surface>>()
    {
        new Tuple<string, Surface> ("Ground", Surface.ground),
        new Tuple<string, Surface> ("Charged Ground", Surface.chargedGround),
        new Tuple<string, Surface> ("Bouncer", Surface.bouncer),
        new Tuple<string, Surface> ("Blower", Surface.blower),
        new Tuple<string, Surface> ("Booster", Surface.booster)
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
    #endregion

    #region Variables
    private static MovementValues groundVals = new MovementValues(40, 8, 12, 30, 29, 0.05f, 5, 0f, true);
    private static MovementValues airVals = new MovementValues(7, 5, 8, 6, 6, 0.05f, 1, 0f, false);

    //default vertical decel values
    private readonly MovementValues verticalDecelVals = new MovementValues(0, 0, 0, 0, 3, 0.1f, 0.5f, 1, false);



    //Movement Physics Constants
    private readonly Dictionary<Surface, MovementValues> surfaceProperties = new()
    {
        {Surface.air,           airVals   },
        {Surface.ground,        groundVals},
        {Surface.chargedGround, groundVals},
        {Surface.bouncer,       groundVals},
        {Surface.blower,        groundVals},
        {Surface.booster,       groundVals}
    };

    //Jumping
    private const float jumpForce = 10f;
    private readonly Vector2 jumpBias = new Vector2(0, 0.8f);

    private Vector2 currentJumpDirection = Vector2.zero;
    private Vector2 lastJumpDirection = Vector2.zero;
    private List<Tuple<Collider2D, Vector2>> savedChargedForces = new();

    //Jump buffer before colliding is not needed very much, compared to buffer after leaving
    private const double jumpBufferBeforeColliding = 0.04;
    private double lastJumpPressedTime;
    private bool jumpQueued = false;

    private double jumpTime = 0;
    private bool heldJump = false;
    private bool jumpedLastFrame = false;

    private const double jumpBufferAfterLeaving = 0.06;
    private bool ableToJumpAfterLeavingGround = false;
    private double lastTimeOnGround;
    private JumpInfo lastGroundJumpInfo;

    //Should these be specific to surfaces?
    private const double minExtraJumpTime = 0.08; //Time after jump when extra jump force starts applying 
    private const double maxExtraJumpTime = 0.4; //Time after jump when extra jump force stops applying
    private const float extraJumpForce = 20f; //per second

    private Surface highestPrioritySurface = Surface.air;
    private Rigidbody2D playerRB;
    private List<Vector2> contactNormals = new();

    private const float gravityScale = 2f;
    #endregion

    public void Initialize()
    {
        playerRB = GetComponent<Rigidbody2D>();
        Time.fixedDeltaTime = (float)1 / 100; //100 fps
        playerRB.gravityScale = gravityScale;

        ResetMovement();
    }

    void FixedUpdate()
    {
        if (jumpedLastFrame)
        {
            highestPrioritySurface = Surface.air;
            jumpedLastFrame = false;
        }

        #region Acceleration
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
        #endregion

        #region Deceleration
        //DECELERATION
        //Update decel function in the future to return negative values if given a negative speed?
        float decelX = CalculateDeceleration(Mathf.Abs(playerRB.velocity.x), surfaceProperties[highestPrioritySurface], false);
        //The y deceleration is a combination of the surface + the default air deceleration
        float decelY = CalculateDeceleration(Mathf.Abs(playerRB.velocity.y), surfaceProperties[highestPrioritySurface], true)
                     + CalculateDeceleration(Mathf.Abs(playerRB.velocity.y), verticalDecelVals, false);

        //First put the decel in the direction opposite the players velocity and calculate for the current frame
        //If over current velocity, and would push the player in the opposite direction make it equal to current velocity, otherwise leave it
        decelX *= -1 * Mathf.Sign(playerRB.velocity.x) * Time.fixedDeltaTime;
        if (Mathf.Abs(decelX) >= Mathf.Abs(playerRB.velocity.x))
        {
            decelX = playerRB.velocity.x * -1;
        }

        decelY *= -1 * Mathf.Sign(playerRB.velocity.y) * Time.fixedDeltaTime;
        if (Mathf.Abs(decelY) >= Mathf.Abs(playerRB.velocity.y))
        {
            decelY = playerRB.velocity.y * -1;
        }

        //Applies the deceleration force
        playerRB.AddForce(new Vector2(decelX, decelY), ForceMode2D.Impulse);
        #endregion

        #region Jumping
        //JUMPING
        //Debug.Log("Frame: " + Time.frameCount + " Touching ground: " + !(highestPrioritySurface == Surface.air));

        //Generate jumping info
        if (surfaceProperties[highestPrioritySurface].canJump)
        {
            //Finds the closest contact normal to vertical
            Vector2 mostVerticalNormal = Vector2.down;

            foreach (Vector2 normal in contactNormals)
            {
                if (normal.y > mostVerticalNormal.y)
                {
                    mostVerticalNormal = normal;
                }
            }
            currentJumpDirection = (mostVerticalNormal.normalized + jumpBias).normalized;

            lastGroundJumpInfo = new JumpInfo(surfaceProperties[highestPrioritySurface], savedChargedForces, currentJumpDirection);

            ableToJumpAfterLeavingGround = true;
            lastTimeOnGround = Time.timeAsDouble;
        }
        //If not pressing jump
        if (!ControlsManager.Instance.Jump)
        {
            heldJump = false;

            //If jumped just before touching the ground
            if(jumpQueued && surfaceProperties[highestPrioritySurface].canJump && (Time.timeAsDouble - lastJumpPressedTime <= jumpBufferBeforeColliding))
            {
                Jump(lastGroundJumpInfo);
            }
            
        }
        //If pressing Jump
        else
        {
            //This order of conditions means that if you have held the spacebar you won't jump again until you release and press it again
            //This can be changed to auto jump after the max force has been given (by setting heldJump to false after the time is up)
            //Or it can be completely removed (by changing the else if to and if)

            //If still holding jump & with time limit then add extra force
            if (heldJump)
            {
                double timeAfterJump = Time.timeAsDouble - jumpTime;
                if (timeAfterJump >= minExtraJumpTime && timeAfterJump <= maxExtraJumpTime)
                {
                    playerRB.AddForce(extraJumpForce * Time.fixedDeltaTime * lastJumpDirection, ForceMode2D.Impulse);
                }
            }
            //If pressing jump but not on the ground
            else if(!surfaceProperties[highestPrioritySurface].canJump)
            {
                if (ableToJumpAfterLeavingGround && (Time.timeAsDouble - lastTimeOnGround <= jumpBufferAfterLeaving))
                {
                    Jump(lastGroundJumpInfo);
                }
                else
                {
                    jumpQueued = true;
                    lastJumpPressedTime = Time.timeAsDouble;
                }
            }
            //If holding jump and able to jump then jump
            else
            {
                Jump(lastGroundJumpInfo);
            }
        }
        #endregion

        //Reset variables for detection between frames
        highestPrioritySurface = Surface.air;
        contactNormals.Clear();
    }



    #region Collision Detection and Calculations
    void OnCollisionEnter2D(Collision2D col)
    {
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

        ContactPoint2D[] contacts = new ContactPoint2D[col.contactCount];
        col.GetContacts(contacts);

        foreach (ContactPoint2D point in contacts)
        {
            contactNormals.Add(point.normal);
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
    #endregion

    #region Other Functions
    /*public void SetKinematic(bool isKinematic)
    {
        playerRB.isKinematic = isKinematic;
    }*/

    private void Jump(JumpInfo info)
    {
        Vector2 force = info.jumpDirection * jumpForce;

        //Add forces from charged walls into jump
        if (info.savedForces.Count > 0)
        {
            foreach (Tuple<Collider2D, Vector2> item in savedChargedForces)
            {
                force += item.Item2;
            }
        }

        playerRB.AddForce(force, ForceMode2D.Impulse);

        heldJump = true;
        jumpTime = Time.timeAsDouble;
        jumpedLastFrame = true;
        Debug.Log("Frame: " + Time.frameCount + " Jump");

        ableToJumpAfterLeavingGround = false;
        jumpQueued = false;

        lastJumpDirection = info.jumpDirection;
    }

    public void ResetMovement()
    {
        playerRB.velocity = Vector2.zero;
        playerRB.angularVelocity = 0;

        jumpedLastFrame = false;
        heldJump = false;
        savedChargedForces.Clear();
        jumpQueued = false;

    //Not needed I think:
        highestPrioritySurface = Surface.air;
        contactNormals.Clear();
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

    private float CalculateAcceleration(float velocity, MovementValues values)
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

            accel = ((maxA - minA) / 2 * Mathf.Cos(Mathf.PI * (velocity - x1) / (x2 - x1))) + ((maxA + minA) / 2);
        }
        else
        {
            accel = values.minAccel;
        }

        return accel;
    }

    private float CalculateDeceleration(float velocity, MovementValues values, bool yDecel)
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
    #endregion

    #region Structs
    private readonly struct JumpInfo
    {
        public readonly List<Tuple<Collider2D, Vector2>> savedForces;
        public readonly MovementValues jumpVariables;
        public readonly Vector2 jumpDirection;

        public JumpInfo(MovementValues jumpSurfaceValues, List<Tuple<Collider2D, Vector2>> savedChargedForces, Vector2 jumpNormal)
        {
            jumpVariables = jumpSurfaceValues;
            savedForces = savedChargedForces;
            jumpDirection = jumpNormal;
        }
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
    #endregion
}
