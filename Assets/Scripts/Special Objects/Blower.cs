using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Blower : MonoBehaviour, IToggleableObject
{
    [SerializeField] private float maxForce;
    [SerializeField] private float minForce;
    [SerializeField] private float maxDistance;
    [SerializeField] private float angleOnUnitCircle;

    private static readonly string activeTag = PlayerMovement.TagFromSurface(PlayerMovement.Surface.blower);
    private static readonly string inActiveTag = PlayerMovement.TagFromSurface(PlayerMovement.Surface.ground);

    static Color DisabledColour = Color.gray;
    static Color BlowerColour = Color.blue;

    private Vector3 forceDirection;

    public bool Activated { get; private set; } = true;

    public void Start()
    {
        forceDirection = new Vector3(Mathf.Cos(angleOnUnitCircle * Mathf.Deg2Rad), Mathf.Sin(angleOnUnitCircle * Mathf.Deg2Rad));
    }

    public void Activate()
    {
        Activated = true;
        GetComponent<SpriteRenderer>().color = BlowerColour;
        tag = activeTag;
    }

    public void Deactivate()
    {
        Activated = false;
        GetComponent<SpriteRenderer>().color = DisabledColour;
        tag = inActiveTag;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Activated)
        {
            BlowObject(other);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (Activated)
        {
            BlowObject(other);
        }
    }

    private void BlowObject(Collider2D obj)
    {
        Vector2 vectorDistance = obj.transform.position - transform.position;
        
        //Find distance in the direction of the blower (vectorDistance is a normalized vector)
        float distance = Vector2.Dot(vectorDistance, forceDirection);

        float force = CalculateForce(distance);

        obj.attachedRigidbody.AddForce(force * Time.fixedDeltaTime * forceDirection, ForceMode2D.Impulse);

    }

    //https://www.desmos.com/calculator/ezkdgbmxhy
    //Calculates force based off of a linear decrease from max to min
    private float CalculateForce(float distance)
    {
        return ((minForce - maxForce) * distance / maxDistance) + maxForce;
    }
}
