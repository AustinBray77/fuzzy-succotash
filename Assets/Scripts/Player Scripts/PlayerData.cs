using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    //Add to set functions later if more complex assignment is needed
    //Or make private and add setter functions separately

    //Turned on after you complete the level or you have died
    //Stops you from triggering the level finish or triggering the death call multiple times
    public bool RespawningState { get; set; } = false;

    public bool Invincible { get; set; } = false;
}
