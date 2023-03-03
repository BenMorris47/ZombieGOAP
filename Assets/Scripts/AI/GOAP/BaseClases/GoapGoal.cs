using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class GoapGoal : MonoBehaviour
{
    protected Dictionary<string, bool> desiredWorldState = new Dictionary<string, bool>();
    public float goalBaseCost = 100;
    public bool requiresReset = false;
    

	public void Awake()
	{
        SetUpGoal();
	}
    public abstract bool CanPerformGoal(GoapAIAgent agent); //checks the goal can be done
    public abstract void SetUpGoal();//sets up all the goals values
    public Dictionary<string, bool> GetGoal(GoapAIAgent agent) //this allows goals to be reset (obsolete after removal of smart goals) then checks the procidural states and returns its desired world state
    {
		if (requiresReset)
		{
            ResetGoal();
		}
        CheckProciduralStates(agent);
        return desiredWorldState;
    }
    protected void AddGoalState(string stateName, bool value) //adds a goal state
	{
        desiredWorldState.Add(stateName, value);
	}
    protected void AddProciduralGoalState(string stateName, bool value)//this can be used to add new states when the goal is evaluated and marks the goal for reset
    {
        desiredWorldState.Add(stateName, value);
        requiresReset = true;

    }

    public virtual void ResetGoal() //wipes the dictionary and sets up the goal again
	{
        desiredWorldState.Clear();
        SetUpGoal();
	}
    public abstract float GetCost(GoapAIAgent agent); //returns the cost allowing it to change to the requirements of the goal

    //this allows procidural states to be created for certain goals return false to not add any new states
    public abstract void CheckProciduralStates(GoapAIAgent agent);

}
