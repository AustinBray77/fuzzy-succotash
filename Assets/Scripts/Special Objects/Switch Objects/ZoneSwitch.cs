using UnityEngine;

public class ZoneSwitch : Switch
{
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag != "Player")
        {
            return;
        }

        Toggle();
    }
}