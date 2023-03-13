using UnityEngine;

public class GameHandler : Singleton<GameHandler>
{
    private void Start()
    {
        /** initilaize all level datas (FUTURE) **/
        SaveHandler.Instance.Load();
    }
}