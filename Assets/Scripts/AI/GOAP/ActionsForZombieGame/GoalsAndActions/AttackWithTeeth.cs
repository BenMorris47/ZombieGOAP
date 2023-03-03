using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackWithTeeth : GoapAction
{
	bool attacked = false;
	public float attackAmount = 5;
	public float intrestDistance = 10;
	public float newInfectionAmount = 15;
	public override bool checkProceduralPrecondition(GoapAIAgent agent)
	{
		HumanoidAgent hAgent = (HumanoidAgent)agent;
		actionTarget = null;
		if (hAgent.stats.isZombie)//if zombie set target to human
		{
			actionTarget = agent.sensor.ClosestObjectWithTagSphereCast("Human", hAgent.stats.viewRadius, true);
		}
		return actionTarget != null;//is there a target
	}

	public override bool IsCompleted()
	{
		return attacked;
	}

	public override bool RequiresInRange()
	{
		return true;
	}

	public override bool RunAction()
	{
		Statistics stats = actionTarget.GetComponent<Statistics>();
		if (Vector3.Distance(transform.position, actionTarget.transform.position) > intrestDistance) //if too far away fail
		{
			return false; //human out of range
		}
		if (inRange) //if in range damage target
		{
			stats.TakeDamage(attackAmount * Time.deltaTime);
			stats.Infect();
			attacked = true;
		}
		if (!stats.isAlive)//if target died complete
		{
			attacked = true;
			return true;
		}
		
		return true;
	}

	public override float GetCost(GoapAIAgent agent)
	{
		//if target isn't infected then the cost to infect goes down significantly 
		return actionTarget.GetComponent<Statistics>().isInfected ? actionBaseCost - attackAmount : actionBaseCost - (attackAmount + newInfectionAmount);
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
