using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = Player.Instance.PlayerPos;
        transform.position = new Vector3(playerPos.x, playerPos.y, transform.position.z);
    }
}
