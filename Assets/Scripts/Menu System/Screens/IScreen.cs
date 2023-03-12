using UnityEngine;

//Interface for menu screens
public interface IScreen
{
    GameObject[] ScreenElements { get; }
    string Name { get; }

    void Initialize();
    void Load();
    void Unload();
}