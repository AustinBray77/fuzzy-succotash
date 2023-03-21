using Mono.CompilerServices.SymbolWriter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

[Serializable]
public class LevelProgresser
{
    //Implement this?
    //https://forum.unity.com/threads/serialized-interface-fields.1238785/

    [Serializable]
    public struct Stage
    {
        [SerializeField]/*[RequireInterface(typeof(IToggleableObject))]*/ private MonoBehaviour[] _activations;
        [SerializeField]/*[RequireInterface(typeof(IToggleableObject))]*/ private MonoBehaviour[] _deactivations;
        public IToggleableObject[] Activations => (IToggleableObject[])_activations;
        public IToggleableObject[] Deactivations => (IToggleableObject[])_deactivations;
    }

    [SerializeField] private Stage[] stages;

    private int numberOfStages;

    public int NumberOfStages { get => numberOfStages; }

    private List<IToggleableObject>[] activeObjects;
    private List<IToggleableObject>[] inactiveObjects;

    //Needed for creating the level data
    public int GetNumberOfStages()
    {
        return (stages is null) ? 0 : stages.Length;
    }

    public void Initialize()
    {
        numberOfStages = (stages is null) ? 0 : stages.Length;
        activeObjects = new List<IToggleableObject>[numberOfStages];
        Debug.Log("Initializing Level Progresser, Active Objects: " + stages[0].Activations.Length + " Disabled Objects: " + stages[0].Deactivations.Length);
        inactiveObjects = new List<IToggleableObject>[numberOfStages];

        for (int i = 0; i < numberOfStages; i++)
        {
            activeObjects[i] = new List<IToggleableObject>();
            inactiveObjects[i] = new List<IToggleableObject>();
        }


        //Creates an array of lists of which objects should be active at each stage (not just which ones change between stages)
        //This way any stage can be loaded directly, without extra steps
        if (stages is not null && stages.Length > 0)
        {

            if ((stages[0].Activations is not null) && stages[0].Activations.Length > 0)
            {
                activeObjects[0].AddRange(stages[0].Activations);
            }
            if ((stages[0].Activations is not null) && stages[0].Deactivations.Length > 0)
            {
                inactiveObjects[0].AddRange(stages[0].Deactivations);
            }
        }

        for (int i = 1; i < numberOfStages; i++)
        {
            activeObjects[i].AddRange(activeObjects[i - 1]);
            inactiveObjects[i].AddRange(inactiveObjects[i - 1]);

            inactiveObjects[i].AddRange(stages[i].Deactivations);
            activeObjects[i].AddRange(stages[i].Activations);

            foreach (IToggleableObject obj in stages[i].Deactivations)
            {
                activeObjects[i].Remove(obj);
            }

            foreach (IToggleableObject obj in stages[i].Activations)
            {
                inactiveObjects[i].Remove(obj);
            }
        }

        Debug.Log("Level Progressor Initialized");
        //Debug.Log(activeObjects.Length);
    }

    public void NextStage(int nextStage)
    {

    }

    public void LoadStage(int stage)
    {
        Debug.Log(activeObjects[stage].Count);
        foreach (IToggleableObject obj in activeObjects[stage])
        {
            obj.Activate();
        }

        Debug.Log(inactiveObjects[stage].Count);
        foreach (IToggleableObject obj in inactiveObjects[stage])
        {
            obj.Deactivate();
        }
    }
}
