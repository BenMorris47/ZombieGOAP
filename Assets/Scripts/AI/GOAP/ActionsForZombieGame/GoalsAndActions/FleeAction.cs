using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeAction : GoapAction
{
	bool fled = false;
	public float extraFleeReach = 5;
	public int fleePosSampleAmount = 30;
	HumanoidAgent hAgent;
	public override bool checkProceduralPrecondition(GoapAIAgent agent)
	{
		actionTarget = agent.waypoint.gameObject;
		hAgent = (HumanoidAgent)agent;
		GameObject enemyTarget = null;
		if (hAgent.stats.isZombie)
		{
			enemyTarget = agent.sensor.ClosestObjectWithTagSphereCast("Human", hAgent.stats.viewRadius, true);
			if (enemyTarget == null)
			{
				return false;
			}
			agent.waypoint.position = agent.sensor.TryFindFleeLocation(enemyTarget.transform.position, hAgent.stats.viewRadius + extraFleeReach, fleePosSampleAmount);
		}
		else
		{
			enemyTarget = agent.sensor.ClosestObjectWithTagSphereCast("Zombie", hAgent.stats.viewRadius, true);
			if (enemyTarget == null)
			{
				return false;
			}
			agent.waypoint.position = agent.sensor.TryFindFleeLocation(enemyTarget.transform.position, hAgent.stats.viewRadius + extraFleeReach, fleePosSampleAmount);
		}
		return true;
	}

	public override bool IsCompleted()
	{
		return fled;
	}

	public override bool RequiresInRange()
	{
		return true;
	}

	public override bool RunAction()
	{
		if (inRange)//if in range
		{
			if (hAgent.stats.AreEnemiesNear())//if any enemies are near then fail otherwise complete
			{
				return false;
			}
			fled = true;
		}
		return true;
	}

	public override void SetUpAction()
	{
		range = 2;
		AddEffect("NearEnemies", false);
	}

	protected override void ResetAction()
	{
		fled = false;
		inRange = false;
	}
}
