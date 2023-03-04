using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

[Serializable]
public class ComponentData
{
    private Dictionary<string, object> _data = new Dictionary<string, object>();

    public void SetValue<T>(string name, T value) where T : ISerializable
        => _data.Add(name, value);

    public T GetValue<T>(string name) where T : ISerializable
        => (T)Convert.ChangeType(_data[name], typeof(T));

}