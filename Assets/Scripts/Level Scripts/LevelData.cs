using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelData : ISaveableComponent
{
    public LevelData(string id, int numberOfStages, int attempts = 0, int deaths = 0, int[] completions = null, double[] bestTimes = null)
    {
        ID = id;
        NumberOfStages = numberOfStages;
        Attempts = attempts;
        Deaths = attempts;
        Completions = completions;
        BestTimes = bestTimes;
    }

    public string ID { get; private set; }
    public int Attempts { get; private set; }
    public int Deaths { get; private set; }
    public int[] Completions { get; private set; }
    public double[] BestTimes { get; private set; }
    public int NumberOfStages { get; private set; }
    public Image Thumbnail { get; private set; }

    public ComponentData Serialize()
    {
        ComponentData data = new ComponentData();

        data.SetValueString("ID", ID);
        data.SetValue<int>("Attempts", Attempts);
        data.SetValue<int>("Deaths", Deaths);
        data.SetArrayValue<int>("Completions", Completions);
        data.SetArrayValue<double>("BestTimes", BestTimes);

        return data;
    }

    public void Deserialize(ComponentData data)
    {
        ID = data.GetValueString("ID");
        Attempts = data.GetValue<int>("Attemps");
        Deaths = data.GetValue<int>("Deaths");
        Completions = data.GetArrayValue<int>("Completions");
        BestTimes = data.GetArrayValue<double>("BestTime");
    }
}

