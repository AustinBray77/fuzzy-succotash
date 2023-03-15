using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveHandler : Singleton<SaveHandler>
{
    private readonly string _saveFilePath = Application.persistentDataPath + "/48p.bin";
    private readonly string _test_file_path = "C:/";

    private Dictionary<string, ISaveableComponent> _saveableComponents;

    //Methods for adding and removing saveable components for when they load in
    public void AddSaveableComponent(ISaveableComponent saveableComponent)
    {
        _saveableComponents.Add(saveableComponent.ID, saveableComponent);
    }

    public void RemoveSaveableComponent(ISaveableComponent saveableComponent)
    {
        _saveableComponents.Remove(saveableComponent.ID);
    }

    //Methods for overwriting data on a possibly existing component, and getting components from ID
    public void SafeOverwriteComponent(ISaveableComponent saveableComponent)
    {
        if (_saveableComponents.ContainsKey(saveableComponent.ID))
        {
            _saveableComponents[saveableComponent.ID] = saveableComponent;
        }

        AddSaveableComponent(saveableComponent);
    }

    public ISaveableComponent GetSaveableComponent(string ID)
    {
        if (_saveableComponents.ContainsKey(ID))
        {
            return _saveableComponents[ID];
        }

        throw new Exception("Invalid Save Component ID");
    }

    public bool SaveableComponentExists(string ID)
    {
        return _saveableComponents.ContainsKey(ID);
    }

    //Method for saving data
    public void Save()
    {

        //Creates the save file if the file doesn't exist
        if (File.Exists(_saveFilePath))
        {
            File.Delete(_saveFilePath);
        }

        //Recreates the save file
        FileStream saveFileStream = File.Create(_saveFilePath);
        BinaryFormatter formatter = new BinaryFormatter();

        //Loops through and serializes each component
        foreach (var currentComponent in _saveableComponents.Values)
        {
            try
            {
                //Serializes the data into the file
                formatter.Serialize(saveFileStream, currentComponent.Serialize());
            }
            catch (Exception e)
            {
                //Logs the error if there is one
                Debug.LogError(e.Message);
            }
        }

        //Closes the file
        saveFileStream.Close();
    }

    //Method for loading data
    public void Load()
    {
        //If the save file isnt found, throw an error
        if (!File.Exists(_saveFilePath))
        {
            throw new Exception("Save File Not Found.");
        }

        //Opens the save file and formatter
        FileStream saveFileStream = File.OpenRead(_saveFilePath);
        BinaryFormatter formatter = new BinaryFormatter();

        //Deserialize the data
        ComponentData[] components = (ComponentData[])formatter.Deserialize(saveFileStream);

        //Reassign each value to its component
        foreach (var currentComponent in components)
        {
            GetSaveableComponent(currentComponent.GetValueString("ID")).Deserialize(currentComponent);
        }

        //Closes the file
        saveFileStream.Close();
    }
}