using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatDeadHumanAction : GoapAction
{
	
	float startTime = 0;
	public int healAmount = 20;
	public float healDuration = 2;
	public bool consumedBody = false;
	private Statistics stats;
	public override bool checkProceduralPrecondition(GoapAIAgent agent)
	{
		actionTarget = agent.sensor.ClosestObjectWithTag("Dead"); //is there a dead body in the scene
		return stats!= null && actionTarget != null && stats.isZombie;//is there a body and agent is a zombie
	}

	public override bool IsCompleted()
	{
		return consumedBody;
	}

	public override float GetCost(GoapAIAgent agent)
	{
		return (base.GetCost(agent) - healAmount) + healDuration; //takes into account how much will be healed but also the time to do so
	}

	public override bool RequiresInRange()
	{
		return true;
	}

	public override bool RunAction()
	{
		if (startTime == 0)//set starting time
		{
			startTime = Time.time;
		}
		else
		{
			if (Time.time - startTime > healDuration)//has the duration for eating completed if so complete
			{
				consumedBody = true;
				stats.Heal(healAmount);
				Destroy(actionTarget);
			}
		}
		return !stats.AreEnemiesNear();//carries out action while no enemies are around
	}

	public override void SetUpAction()
	{
		stats = GetComponent<Statistics>();
		AddPrecondition("NearEnemies", false);
		AddEffect("Healed", true);
	}

	protected override void ResetAction()
	{
		startTime = 0;
		actionTarget = null;
		consumedBody = false;
	}
}
