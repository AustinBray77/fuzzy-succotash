using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveHandler : Singleton<SaveHandler>
{
    private readonly string _saveFilePath = Application.persistentDataPath + "/48p.bin";
    private readonly string _test_file_path = "C:/";

    private Dictionary<int, SaveableComponent> _saveableComponents;

    //Methods for adding and removing saveable components for when they load in
    public void AddSaveableComponent(SaveableComponent sc)
    {
        _saveableComponents.Add(sc.ID, sc);
    }

    public void RemoveSaveableComponent(SaveableComponent sc)
    {
        _saveableComponents.Remove(sc.ID);
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
        FileStream fs = File.Create(_test_file_path);
        BinaryFormatter formatter = new BinaryFormatter();

        //Loops through and serializes each component
        foreach (var component in _saveableComponents.Values)
        {
            try
            {
                //Serializes the data into the file
                formatter.Serialize(fs, component.Serialize());
            }
            catch (Exception e)
            {
                //Logs the error if there is one
                Debug.LogError(e.Message);
            }
        }


        fs.Close();
    }

    //Method for loading data
    public void Load()
    {

    }
}