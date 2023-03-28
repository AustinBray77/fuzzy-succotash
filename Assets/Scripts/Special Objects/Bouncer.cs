using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bouncer : MonoBehaviour, IToggleableObject
{
    public bool Activated { get; private set; } = true;
    private static readonly string activeTag = PlayerMovement.TagFromSurface(PlayerMovement.Surface.bouncer);
    private static readonly string inActiveTag = PlayerMovement.TagFromSurface(PlayerMovement.Surface.ground);

    [SerializeField] private float bounceFactor = 1;
    [SerializeField] private float addedForce = 0;

    static Color DisabledColour = Color.gray;
    static Color BouncerColour = Color.yellow;

    public void Activate()
    {
        Activated = true;
        GetComponent<SpriteRenderer>().color = BouncerColour;
        tag = activeTag;
    }

    public void Deactivate()
    {
        Activated = false;
        GetComponent<SpriteRenderer>().color = DisabledColour;
        tag = inActiveTag;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (Activated)
        {
            ContactPoint2D[] contacts = new ContactPoint2D[col.contactCount];
            col.GetContacts(contacts);
            Vector2 avgNormal = Vector2.zero;
            foreach (ContactPoint2D point in contacts)
            {
                avgNormal += point.normal;
            }
            avgNormal = avgNormal.normalized * -1;

            //Vector2.Dot gives the magnitude of the velocity in the direction of the average normal
            float force = (Mathf.Abs(Vector2.Dot(avgNormal, col.relativeVelocity)) * bounceFactor) + addedForce;

            col.rigidbody.AddForce(force * avgNormal, ForceMode2D.Impulse);
        }

        /*
        float angle = AngleBetweenVectors(avgNormal, -col.relativeVelocity);
        Debug.Log(avgNormal + " " + -col.relativeVelocity  + " " + angle * Mathf.Rad2Deg);
        
        Vector2 finalVector;
        if (angle == 0)
        {
            finalVector = -col.relativeVelocity;
        }
        else if(AToTheLeft(avgNormal, -col.relativeVelocity))
        {
            finalVector = RotateVector(-col.relativeVelocity, 2 * angle);
        }
        else
        {
            finalVector = RotateVector(-col.relativeVelocity, -2 * angle);
        }

        finalVector += finalVector.normalized * forceAdded;

        Debug.Log(finalVector);
        col.rigidbody.velocity = finalVector;
        */


    }

    /*
    //angle in radians
    private float AngleBetweenVectors(Vector2 a, Vector2 b)
    {
        return Mathf.Acos(Vector2.Dot(a, b) / (a.magnitude * b.magnitude));
    }

    //var dot = a.x*-b.y + a.y*b.x;
    private bool AToTheLeft(Vector2 a, Vector2 b)
    {
        float dot = a.x * -b.y + a.y * b.x;
        return dot > 0;
    }

    //Angle in radians, rotation counter-clockwise / to the left
    private Vector2 RotateVector(Vector2 v, float angle)
    {
        return new Vector2((Mathf.Cos(angle) * v.x) - (Mathf.Sin(angle) * v.y), (Mathf.Sin(angle) * v.x) + (Mathf.Cos(angle) * v.y));
    }
    */
}
