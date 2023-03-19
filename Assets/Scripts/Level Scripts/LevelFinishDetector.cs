using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Collider2D))]
public class LevelFinishDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Player.PlayerTag) && !Player.Instance.Data.RespawningState)
        {
            LevelHandler.Instance.CurrentLevelController.LevelCompleted();
        }
    }


}
