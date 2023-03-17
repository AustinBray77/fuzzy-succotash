using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelData : ISaveableComponent
{
    public LevelData(string id, int numberOfStages, string title, int index, int[] attempts = null, int[] deaths = null, int[] completions = null, double[] bestTimes = null)
    {
        ID = id;
        NumberOfStages = numberOfStages;
        Title = title;
        Index = index;
        Attempts = attempts ?? (new int[numberOfStages]);
        Deaths = deaths ?? (new int[numberOfStages]);
        Completions = completions ?? (new int[numberOfStages]);
        BestTimes = bestTimes ?? (new double[numberOfStages]);
    }

    public string ID { get; private set; }
    public int[] Attempts { get; private set; }
    public int[] Deaths { get; private set; }
    public int[] Completions { get; private set; }
    public double[] BestTimes { get; private set; }
    public int NumberOfStages { get; private set; }
    public Image Thumbnail { get; private set; }
    public string Title { get; private set; }
    public int Index { get; private set; }

    public void LogLevelCompletion(int stage, double time)
    {
        Completions[stage]++;

        if (BestTimes[stage] > time || Completions[stage] == 1)
        {
            BestTimes[stage] = time;
        }
    }

    public void LogRespawn(int stage, LevelController.RespawnInfo info)
    {
        switch (info)
        {
            case LevelController.RespawnInfo.playerDied:
                Deaths[stage]++;
                break;
        }
    }

    public void LogAttemptStart(int stage)
    {
        Attempts[stage]++;
    }

    public ComponentData Serialize()
    {
        ComponentData data = new ComponentData();

        data.SetValueString("ID", ID);
        data.SetArrayValue<int>("Attempts", Attempts);
        data.SetArrayValue<int>("Deaths", Deaths);
        data.SetArrayValue<int>("Completions", Completions);
        data.SetArrayValue<double>("BestTimes", BestTimes);

        return data;
    }

    public void Deserialize(ComponentData data)
    {
        ID = data.GetValueString("ID");
        Attempts = data.GetArrayValue<int>("Attemps");
        Deaths = data.GetArrayValue<int>("Deaths");
        Completions = data.GetArrayValue<int>("Completions");
        BestTimes = data.GetArrayValue<double>("BestTime");
    }
}

