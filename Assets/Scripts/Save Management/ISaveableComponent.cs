using UnityEngine;

public interface ISaveableComponent
{
    int ID { get; }
    ComponentData Serialize();
    void Deserialize(ComponentData data);
}