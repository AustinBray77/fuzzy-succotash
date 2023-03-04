using UnityEngine;

public abstract class SaveableComponent : MonoBehaviour
{
    protected void Start()
    {
        SaveHandler.Instance.AddSaveableComponent(this);
    }

    protected void OnDestroy()
    {
        SaveHandler.Instance.RemoveSaveableComponent(this);
    }

    public int ID { get; }

    public abstract ComponentData Serialize();

    public abstract void Deserialize(ComponentData data);
}