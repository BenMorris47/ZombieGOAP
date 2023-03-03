using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GoapAction : MonoBehaviour
{
    public Dictionary<string, bool> preConditions { get; private set; } = new Dictionary<string, bool>();
    public Dictionary<string, bool> effects { get; private set; } = new Dictionary<string, bool>();
    public float actionBaseCost = 100;
    public GameObject actionTarget;
    public bool inRange = false;
    public float range = 1f;
    private bool requiresReset = false;
    public void Awake() //Used to set up the actions
    {
        SetUpAction();
    }

    public void ResetActionValues() //Used to wipe all values and reset them. Now obsolete as smart goals and actions have been removed 
    {
        if (requiresReset)
        {
            preConditions.Clear();
            effects.Clear();
            SetUpAction();
        }
        ResetAction();
    }

    protected abstract void ResetAction(); //This resets action values

    public abstract bool IsCompleted();//Is the action finished

    public abstract void SetUpAction();//Called on awake to set up action

    public abstract bool RunAction();//runs the action returning true if it runs correctly or false if not casuing a plan abort

    public abstract bool RequiresInRange();//does this action need to be in range;

    public abstract bool checkProceduralPrecondition(GoapAIAgent agent);//Checks the preconditions of the action if it is posible it will return true and be allowed into the useable actions list in the planner

    public void AddPrecondition(string condition, bool value)//Allows preconditions to be easily added on setup
    {
        preConditions.Add(condition, value);
    }

    public void AddEffect(string condition, bool value)//Allows effects to be easily added on setup
    {
        effects.Add(condition, value);
    }

    public virtual float GetCost(GoapAIAgent agent)//gets the cost which can be overwitten to be changed while planning
    {
        return actionBaseCost;
    }
}
