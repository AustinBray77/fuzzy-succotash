using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

[Serializable]
public class ComponentData
{
    private Dictionary<string, object> _savedData = new Dictionary<string, object>();

    public void SetValue<T>(string name, T value) where T : struct
        => _savedData.Add(name, value);

    public void SetValueString(string name, string value)
        => _savedData.Add(name, value);

    public T GetValue<T>(string name) where T : struct
        => (T)Convert.ChangeType(_savedData[name], typeof(T));

    public string GetValueString(string name)
        => (string)Convert.ChangeType(_savedData[name], typeof(string));

    public void SetArrayValue<T>(string name, T[] value) where T : struct
        => _savedData.Add(name, value);

    public T[] GetArrayValue<T>(string name) where T : struct
        => (T[])Convert.ChangeType(_savedData[name], typeof(T[]));
}