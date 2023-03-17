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
    private struct Stage
    {
        [SerializeField][RequireInterface(typeof(IToggleableObject))] private MonoBehaviour[] _activations;
        [SerializeField][RequireInterface(typeof(IToggleableObject))] private MonoBehaviour[] _deactivations;
        public IToggleableObject[] Activations { get => _activations as IToggleableObject[]; }
        public IToggleableObject[] Deactivations { get => _deactivations as IToggleableObject[]; }
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

        IToggleableObject[] activations;
        IToggleableObject[] deactivations; 

        //Creates an array of lists of which objects should be active at each stage (not just which ones change between stages)
        //This way any stage can be loaded directly, without extra steps
        if (stages.Length > 0)
        {
            activations = stages[0].Activations;
            deactivations = stages[0].Deactivations;
            
            if (activations.Length > 0)
            {
                activeObjects[0].AddRange(activations);
            }
            if (deactivations.Length > 0)
            {
                inactiveObjects[0].AddRange(deactivations);
            }
        }

        for (int i = 1; i < numberOfStages && i < stages.Length; i++)
        {
            activations = (IToggleableObject[]) stages[i].Activations;
            deactivations = (IToggleableObject[]) stages[i].Deactivations;

            activeObjects[i].AddRange(activeObjects[i-1]);
            inactiveObjects[i].AddRange(inactiveObjects[i-1]);

            inactiveObjects[i].AddRange(deactivations);
            activeObjects[i].AddRange(activations);

            foreach (IToggleableObject obj in stages[i].Deactivations)
            {
                activeObjects[i].Remove(obj);
            }

            foreach (IToggleableObject obj in stages[i].Activations)
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
