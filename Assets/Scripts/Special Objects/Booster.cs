using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Booster : MonoBehaviour, IToggleableObject
{
    [SerializeField] private float _totalBoost;

    public bool Activated { get; private set; } = true;
    private Vector2 _addForce;

    private static readonly string activeTag = PlayerMovement.TagFromSurface(PlayerMovement.Surface.booster);
    private static readonly string inActiveTag = PlayerMovement.TagFromSurface(PlayerMovement.Surface.ground);

    static Color DisabledColour = Color.gray;
    static Color BoosterColour = Color.blue;


    private void Start()
    {
        float angle = transform.eulerAngles.z;

        _addForce = new Vector2(
            _totalBoost * Mathf.Cos(angle * Mathf.Deg2Rad),
            _totalBoost * Mathf.Sin(angle * Mathf.Deg2Rad)
            );
    }

    public void Activate()
    {
        Activated = true;
        GetComponent<SpriteRenderer>().color = BoosterColour;
        tag = activeTag;
    }

    public void Deactivate()
    {
        Activated = false;
        GetComponent<SpriteRenderer>().color = DisabledColour;
        tag = inActiveTag;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (Activated && other.gameObject.tag == "Player")
        {
            BoostObject(other.gameObject);
        }
    }

    private void BoostObject(GameObject gameObject)
    {
        Rigidbody2D rigidbody2D = gameObject.GetComponent<Rigidbody2D>();

        if (rigidbody2D == null)
        {
            return;
        }

        rigidbody2D.AddForce(_addForce, ForceMode2D.Impulse);
    }
}
