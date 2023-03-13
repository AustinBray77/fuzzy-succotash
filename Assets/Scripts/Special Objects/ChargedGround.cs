using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ChargedGround : MonoBehaviour, IToggleableObject
{
    public bool Activated { get; private set; } = true;

    private static readonly string activeTag = PlayerMovement.TagFromSurface(PlayerMovement.Surface.chargedGround);
    private static readonly string inactiveTag = PlayerMovement.TagFromSurface(PlayerMovement.Surface.ground);

    static Color DisabledColour = Color.gray;
    static Color ChargedColour = Color.magenta;

    public void Activate()
    {
        GetComponent<SpriteRenderer>().color = ChargedColour;
        tag = activeTag;
    }

    public void Deactivate()
    {
        GetComponent<SpriteRenderer>().color = DisabledColour;
        tag = inactiveTag;
    }
}
