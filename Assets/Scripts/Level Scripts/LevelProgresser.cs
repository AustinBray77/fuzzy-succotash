using Mono.CompilerServices.SymbolWriter;
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

    [SerializeField] private Stage[] stages;

    [SerializeField] private int numberOfStages;
    
    public int NumberOfStages { get => numberOfStages; }

    private List<ToggleableObject>[] activeObjects;
    private List<ToggleableObject>[] inactiveObjects;

    public void Initialize()
    {
        activeObjects = new List<ToggleableObject>[numberOfStages];
        inactiveObjects = new List<ToggleableObject>[numberOfStages];

        //Creates an array of lists of which objects should be active at each stage (not just which ones change between stages)
        //This way any stage can be loaded directly, without extra steps
        activeObjects[0].AddRange(stages[0].activations);
        inactiveObjects[0].AddRange(stages[0].dectivations);

        for (int i = 1; i < numberOfStages; i++)
        {
            activeObjects[i].AddRange(activeObjects[i-1]);
            inactiveObjects[i].AddRange(inactiveObjects[i-1]);

            inactiveObjects[i].AddRange(stages[i].dectivations);
            activeObjects[i].AddRange(stages[i].activations);

            foreach (ToggleableObject obj in stages[i].dectivations)
            {
                activeObjects[i].Remove(obj);
            }

            foreach (ToggleableObject obj in stages[i].activations)
            {
                inactiveObjects[i].Remove(obj);
            }
        }
    }

    public void NextStage()
    {

    }

    public void LoadStage(int stage)
    {

    }
}
