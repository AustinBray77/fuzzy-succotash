using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Collider2D))]
public class LevelFinishDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Level Finish Triggered, player: " + collision.gameObject.CompareTag(Player.PlayerTag) + " Respawning state: " + Player.Instance.Data.RespawningState);

        if (collision.gameObject.CompareTag(Player.PlayerTag) && !Player.Instance.Data.RespawningState)
        {
            LevelHandler.Instance.CurrentLevelController.LevelCompleted();
        }
    }
}
