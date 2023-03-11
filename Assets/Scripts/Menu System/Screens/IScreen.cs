using UnityEngine;

//Interface for menu screens
public interface IScreen
{
    GameObject[] ScreenElements { get; set; }
    string Name { get; set; }

    void Initialize();
    void Load();
    void Unload();
}