using UnityEngine;

public class ZoneSwitch : Switch
{
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            Toggle();
        }
    }
}