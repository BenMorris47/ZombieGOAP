using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackWithFists : GoapAction
{
	bool attacked = false;
	public float attackAmount = 10;
	public float intrestDistance = 10;
	public override bool checkProceduralPrecondition(GoapAIAgent agent)
	{
		HumanoidAgent hAgent = (HumanoidAgent)agent;
		if (hAgent.stats.isZombie) //if zombie then find human
		{
			actionTarget = agent.sensor.ClosestObjectWithTagSphereCast("Human", hAgent.stats.viewRadius, true);
		}
		else//if human then find zombie
		{
			actionTarget = agent.sensor.ClosestObjectWithTagSphereCast("Zombie", hAgent.stats.viewRadius, true);
		}
		return actionTarget != null;//is there a target?
	}

	public override bool IsCompleted()
	{
		return attacked;
	}

	public override bool RequiresInRange()
	{
		return true;
	}
	public override float GetCost(GoapAIAgent agent)
	{
		return base.GetCost(agent) - attackAmount;//takes the amount of damage caused into account
	}

	public override bool RunAction()
	{
		Statistics stats = actionTarget.GetComponent<Statistics>();
		if (Vector3.Distance(transform.position, actionTarget.transform.position) > intrestDistance)//is target inside max range otherwise fail
		{
			return false; //human out of range
		}
		if (inRange)//when in range attack
		{
			stats.TakeDamage(attackAmount * Time.deltaTime);
			attacked = true;
		}
		if (!stats.isAlive)//if target dies complete
		{
			attacked = true;
			return true;
		}
		
		return true;
	}

	public override void SetUpAction()
	{
		range = 1.5f;
		AddPrecondition("NearEnemies", true);
		AddEffect("Attacked", true);
	}

	protected override void ResetAction()
	{
		attacked = false;
		inRange = false;
		actionTarget = null;
	}
}
