using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelProgresser
{
    [Serializable]
    private struct Stage
    {
        [SerializeField] public ToggleableObject[] activations;
        [SerializeField] public ToggleableObject[] dectivations;
    }

    [SerializeField] private Stage[] Stages;

    public int NumberOfStages { get => numberOfStages; }

    private int numberOfStages;

    

    void Initialize()
    {

    }

}
