using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectWoodGoal : GoapGoal
{
	public override float GetCost(GoapAIAgent agent)
	{
		return goalBaseCost;
	}

	public override void CheckProciduralStates(GoapAIAgent agent)
	{
		
	}

	public override void SetUpGoal()//sets up the goal
    {
        AddGoalState("hasWood", true);
    }

	public override bool CanPerformGoal(GoapAIAgent agent)
	{
		return true; //we don't care who can do it
	}
}
