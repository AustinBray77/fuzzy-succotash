using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelData : ISaveableComponent
{
    //Is this needed if data is always created from scratch?
    /*
    public LevelData(string id, int numberOfStages, string title, int index, int[] attempts = null, int[] deaths = null, int[] completions = null, double[] bestTimes = null)
    {
        ID = id;
        NumberOfStages = numberOfStages;
        Title = title;
        Index = index;
        _attempts = attempts ?? (new int[numberOfStages]);
        _deaths = deaths ?? (new int[numberOfStages]);
        _completions = completions ?? (new int[numberOfStages]);
        _bestTimes = bestTimes ?? (new double[numberOfStages]);
    }
    */

    public LevelData(string id, int numberOfStages, string title, int index)
    {
        ID = id;
        NumberOfStages = numberOfStages;
        Title = title;
        Index = index;
        _attempts = new int[numberOfStages];
        _deaths = new int[numberOfStages];
        _completions = new int[numberOfStages];
        _bestTimes = new double[numberOfStages];

        //Readonly collections are needed because otherwise individual values can be written to
        //These collections will update if the underlying list is changed
        Attempts = new ReadOnlyCollection<int>(_attempts);
        Deaths = new ReadOnlyCollection<int>(_deaths);
        Completions = new ReadOnlyCollection<int>(_completions);
        BestTimes = new ReadOnlyCollection<double>(_bestTimes);
    }

    //Keeping private copies stops other classes from modifying these variables
    private int[] _attempts;
    private int[] _deaths;
    private int[] _completions;
    private double[] _bestTimes;

    public string ID { get ; private set; }
    public ReadOnlyCollection<int> Attempts { get; private set; }
    public ReadOnlyCollection<int> Deaths { get; private set; }
    public ReadOnlyCollection<int> Completions { get; private set; }
    public ReadOnlyCollection<double> BestTimes { get; private set; }
    public Image Thumbnail { get; private set; }
    public int NumberOfStages { get; private set; }
    public string Title { get; private set; }
    public int Index { get; private set; }

    public void LogLevelCompletion(int stage, double time)
    {
        _completions[stage]++;

        if (_bestTimes[stage] > time || _completions[stage] == 1)
        {
            _bestTimes[stage] = time;
        }
    }

    public void LogRespawn(int stage, LevelController.RespawnInfo info)
    {
        switch (info)
        {
            case LevelController.RespawnInfo.playerDied:
                _deaths[stage]++;
                break;
        }
    }

    public void LogAttemptStart(int stage)
    {
        _attempts[stage]++;
    }

    //Format data into saveable item
    public ComponentData Serialize()
    {
        ComponentData data = new ComponentData();

        data.SetValueString("ID", ID);
        data.SetArrayValue<int>("Attempts", _attempts);
        data.SetArrayValue<int>("Deaths", _deaths);
        data.SetArrayValue<int>("Completions", _completions);
        data.SetArrayValue<double>("BestTimes", _bestTimes);

        return data;
    }

    //Get data from saved componentData
    public void Deserialize(ComponentData data)
    {
        ID = data.GetValueString("ID");
        _attempts = data.GetArrayValue<int>("Attemps");
        _deaths = data.GetArrayValue<int>("Deaths");
        _completions = data.GetArrayValue<int>("Completions");
        _bestTimes = data.GetArrayValue<double>("BestTime");
    }
}

