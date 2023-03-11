using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

[Serializable]
public class ComponentData
{
    private Dictionary<string, object> _savedData = new Dictionary<string, object>();

    public void SetValuePrimitive<T>(string name, T value) where T : struct
        => _savedData.Add(name, value);

    public void SetValue<T>(string name, T value) where T : ISerializable
        => _savedData.Add(name, value);

    public T GetValuePrimitive<T>(string name) where T : struct
        => (T)Convert.ChangeType(_savedData[name], typeof(T));

    public T GetValue<T>(string name) where T : ISerializable
        => (T)Convert.ChangeType(_savedData[name], typeof(T));

}