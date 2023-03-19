using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Switch : MonoBehaviour, IToggleableObject
{
    public bool Activated { get; private set; }

    private static Color SwitchColor = Color.green;
    private static Color DisabledColor = Color.grey;

    [SerializeField][RequireInterface(typeof(IToggleableObject))] private MonoBehaviour[] _associatedObjects;

    public void Activate()
    {
        Activated = true;
        GetComponent<SpriteRenderer>().color = SwitchColor;
    }

    public void Deactivate()
    {
        Activated = false;
        GetComponent<SpriteRenderer>().color = DisabledColor;
    }

    public void Toggle()
    {
        foreach (IToggleableObject toggleableObject in (_associatedObjects as IToggleableObject[]))
        {
            if (toggleableObject.Activated)
            {
                toggleableObject.Deactivate();
            }
            else
            {
                toggleableObject.Activate();
            }
        }
    }
}
