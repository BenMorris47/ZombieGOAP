using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoapGoalManager : MonoBehaviour
{
	public GoapGoal defaultGoal;
	public bool useDefaultGoal = true;
    public List<GoapGoal> onBoardGoals;
	public List<GoapGoal> externalSmartGoals;
	public void Start()
	{
		SetUpGoals();
	}

	private void SetUpGoals() //used to set up goals and populate goal list
	{
		onBoardGoals = new List<GoapGoal>();
		var goals = GetComponents<GoapGoal>();
		foreach (var goal in goals)
		{
			onBoardGoals.Add(goal);
		}
	}

	public GoapGoal GetGoal(GoapAIAgent agent) //find the cheapest goal in onboard goals as well as setting a default goal if required
	{
		GoapGoal cheapestGoal = useDefaultGoal ? defaultGoal : null; //sets default or null for starting goal
		float cheapestCost = useDefaultGoal ? defaultGoal.GetCost(agent) : 0;
		foreach (var goal in onBoardGoals) //loop through all goals comparing costs to find the cheapest
		{
			if (goal.CanPerformGoal(agent))
			{
				float goalCost = goal.GetCost(agent);
				if (cheapestGoal == null )
				{
					cheapestGoal = goal;
					cheapestCost = goalCost;
				}
				if (goalCost < cheapestCost)
				{
					cheapestGoal = goal;
					cheapestCost = goalCost;
				}
			}
			
		}
		return cheapestGoal;//return the cheapest goal
	}
}
