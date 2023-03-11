using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData
{ 
    public LevelData(int attempts = 0, int deaths = 0, int completions = 0, double? bestTime = null)
    {
        Attempts = attempts;
        Deaths = attempts;
        Completions = completions;
        BestTime = bestTime;
    }

    public int Attempts { get; private set; }
    public int Deaths { get; private set; }
    public int Completions { get; private set; }
    public double? BestTime { get; private set; }
}

