using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleGoal : GoapGoal
{
	public override bool CanPerformGoal(GoapAIAgent agent)
	{
		return true;
	}

	public override void CheckProciduralStates(GoapAIAgent agent)
	{
		
	}

	public override float GetCost(GoapAIAgent agent)
	{
		return goalBaseCost - 1;//very high score so veryh easy to be overriden by other goals
	}

	public override void SetUpGoal()
	{
		AddGoalState("Idled", true);
	}
}
