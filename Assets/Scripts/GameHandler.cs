using UnityEngine;

public class GameHandler : Singleton<GameHandler>
{
    private void Start()
    {
        ControlsManager.Instance.Initialize(); //setup of input actions
        LevelHandler.Instance.Initialize(); //Level handler relies on controls being set to add callbacks for inputs

        for (int i = 0; i < LevelHandler.Instance.LevelReferences.Length; i++)
        {
            LevelHandler.Instance.LevelReferences[i].Initialize(i);
        }

        try
        {
            SaveHandler.Instance.Load();
        }
        catch
        {
            Debug.LogWarning("No Save File Found, All Save Data is Default");
        }
    }
}