using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DamageObject : MonoBehaviour, IToggleableObject
{
    public bool Activated { get; private set; }

    private static readonly string inactiveTag = PlayerMovement.TagFromSurface(PlayerMovement.Surface.ground);
    private static readonly string activeTag = "Damage";
    private static Color DamageColor = Color.red;
    private static Color DisabledColor = Color.grey;

    public void Activate()
    {
        Activated = true;
        GetComponent<SpriteRenderer>().color = DamageColor;
        tag = activeTag;
    }

    public void Deactivate()
    {
        Activated = false;
        GetComponent<SpriteRenderer>().color = DisabledColor;
        tag = inactiveTag;
    }
}
