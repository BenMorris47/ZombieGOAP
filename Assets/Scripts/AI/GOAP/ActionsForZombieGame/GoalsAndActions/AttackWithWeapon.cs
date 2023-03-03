using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackWithWeapon : GoapAction
{
	bool attacked = false;
	public float attackAmount = 15;
	public float intrestDistance = 10;
	public float weaponRange = 2;
	public override bool checkProceduralPrecondition(GoapAIAgent agent)
	{
		HumanoidAgent hAgent = (HumanoidAgent)agent;
		if (!hAgent.stats.isZombie)//if human find zombie
		{
			actionTarget = agent.sensor.ClosestObjectWithTagSphereCast("Zombie", hAgent.stats.viewRadius, true);
		}
		else if (hAgent.stats.isZombie)//if zombie find human
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
	public override float GetCost(GoapAIAgent agent)
	{
		return base.GetCost(agent) - attackAmount;//cost is based on how good the weapon is
	}

	public override bool RunAction()
	{
		Statistics stats = actionTarget.GetComponent<Statistics>();
		if (Vector3.Distance(transform.position, actionTarget.transform.position) > intrestDistance) //if target runs too far away fail
		{
			return false; //human out of range
		}
		if (inRange)//if in range then attack
		{
			stats.TakeDamage(attackAmount * Time.deltaTime);
			attacked = true;
		}
		if (!stats.isAlive)//if the target dies complete
		{
			attacked = true;
			return true;
		}
		
		return true;
	}

	public override void SetUpAction()
	{
		range = weaponRange;
		AddPrecondition("NearEnemies", true);
		AddEffect("Attacked", true);
		AddPrecondition("HasWeapon", true);
	}

	protected override void ResetAction()
	{
		attacked = false;
		inRange = false;
		actionTarget = null;
	}
}
