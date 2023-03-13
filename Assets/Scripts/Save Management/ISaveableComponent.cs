using UnityEngine;

public interface ISaveableComponent
{
    string ID { get; }
    ComponentData Serialize();
    void Deserialize(ComponentData data);
}