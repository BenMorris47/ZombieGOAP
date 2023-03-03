using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealGoal : GoapGoal
{
	HumanoidAgent hAgent;
	public override bool CanPerformGoal(GoapAIAgent agent)
	{
		hAgent = (HumanoidAgent)agent;
		if (hAgent.stats.GetCurrentHealth() >= hAgent.stats.GetMaxHealth()) //return false if at max health
		{
			return false;
		}
		if (hAgent.stats.isZombie) //if agent is a zombie
		{
			if (agent.sensor.AmountObjectsWithTag("Dead") > 0)//if there are bodies around return true
			{
				return true;
			}
		}
		else //if human
		{
			if (agent.sensor.AmountObjectsWithTag("Medpack") > 0 || hAgent.stats.inv.doesHaveItem("Medpack")) //are there any medpacks in the world or does the agent have one
			{
				return true;
			}
		}
		return false;
	}

	public override void CheckProciduralStates(GoapAIAgent agent)
	{
		
	}

	public override float GetCost(GoapAIAgent agent)
	{
		//as the agent health goes lower the difference gets bigger which is removed from the score making it cheaper as agent health lowers
		return goalBaseCost -((hAgent.stats.GetMaxHealth()/2)- hAgent.stats.GetCurrentHealth() / 2);
	}

	public override void SetUpGoal()
	{
		AddGoalState("NearEnemies", false);
		AddGoalState("Healed", true);
	}
}
