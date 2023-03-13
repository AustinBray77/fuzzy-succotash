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
        [SerializeField] public IToggleableObject[] activations;
        [SerializeField] public IToggleableObject[] dectivations;
    }

    [SerializeField] private Stage[] stages;

    [SerializeField] private int numberOfStages;
    
    public int NumberOfStages { get => numberOfStages; }

    private List<IToggleableObject>[] activeObjects;
    private List<IToggleableObject>[] inactiveObjects;

    public void Initialize()
    {
        activeObjects = new List<IToggleableObject>[numberOfStages];
        inactiveObjects = new List<IToggleableObject>[numberOfStages];

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

            foreach (IToggleableObject obj in stages[i].dectivations)
            {
                activeObjects[i].Remove(obj);
            }

            foreach (IToggleableObject obj in stages[i].activations)
            {
                inactiveObjects[i].Remove(obj);
            }
        }
    }

    public void NextStage(int newStage)
    {

    }

    public void LoadStage(int stage)
    {
        foreach (IToggleableObject obj in activeObjects[stage])
        {
            obj.Activate();
        }

        foreach (IToggleableObject obj in inactiveObjects[stage])
        {
            obj.Deactivate();
        }
    }
}
