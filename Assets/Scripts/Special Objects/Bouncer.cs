using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof (Collider2D))]
public class Bouncer : MonoBehaviour
{
    private bool enabled = false;
    [SerializeField] private float forceAdded;
    Collider2D collider;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D (Collision2D col)
    {
        ContactPoint2D[] contacts = new ContactPoint2D[col.contactCount];
        col.GetContacts(contacts);
        Vector2 avgNormal = Vector2.zero;
        foreach (ContactPoint2D point in contacts)
        {
            avgNormal += point.normal;
        }
        avgNormal = avgNormal.normalized * -1;

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
    }

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
}
