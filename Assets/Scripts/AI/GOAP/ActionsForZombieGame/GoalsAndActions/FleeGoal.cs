using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeGoal : GoapGoal
{
	HumanoidAgent hAgent;
	public int fleeOutNumberThreshold = 4;
	public float healthFleePercent = 0.25f;
	public int enemyAmount = 0;
	public int friendlyAmount = 0;
	public float costPerEnemy = 5;
	public override bool CanPerformGoal(GoapAIAgent agent)
	{
		hAgent = (HumanoidAgent)agent;
		//set up friendly and enemy amounts
		if (hAgent.stats.isZombie)
		{
			enemyAmount = agent.sensor.AmountObjectsWithTag("Human");
			friendlyAmount = agent.sensor.AmountObjectsWithTag("Zombie");
			
		}
		else
		{
			friendlyAmount = agent.sensor.AmountObjectsWithTag("Human");
			enemyAmount = agent.sensor.AmountObjectsWithTag("Zombie");
		}
		//if health drops below the health flee percent and the amount of enemies outways the friendly agents
		if (hAgent.stats.GetMaxHealth()/hAgent.stats.GetCurrentHealth() <= healthFleePercent && friendlyAmount - enemyAmount < fleeOutNumberThreshold)
		{
			return true;
		}
		return false;
	}

	public override void CheckProciduralStates(GoapAIAgent agent)
	{
		
	}

	public override float GetCost(GoapAIAgent agent)
	{
		return goalBaseCost - (costPerEnemy * enemyAmount);//as more enemies get close the cost goes down
	}

	public override void SetUpGoal()
	{
		AddGoalState("NearEnemies", false);
	}
}
