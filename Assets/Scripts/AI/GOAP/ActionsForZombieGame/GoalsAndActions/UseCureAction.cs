using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseCureAction : GoapAction
{
	
	float startTime = 0;
	public int healAmount = 30;
	public float healDuration = 7;
	public bool usedCure = false;
	private Statistics stats;
	public override bool checkProceduralPrecondition(GoapAIAgent agent)
	{
		return stats!= null; //just makes sure that there are stats 
	}

	public override bool IsCompleted()
	{
		return usedCure;
	}

	public override float GetCost(GoapAIAgent agent)
	{
		return (base.GetCost(agent) - healAmount) + healDuration; //takes into account how much will be healed but also the time to do so
	}

	public override bool RequiresInRange()
	{
		return false;//can perform anywhere
	}

	public override bool RunAction()
	{
		if (startTime == 0) //the stime that it is first run
		{
			startTime = Time.time;
		}
		else
		{
			if (Time.time - startTime > healDuration) //has the duration of time elapsed if so cure the agent
			{
				usedCure = true;
				stats.Heal(healAmount);
				stats.Cure();
				stats.inv.RemoveItem("Cure");
			}
		}
		return !stats.AreEnemiesNear();//carries out action while no enemies are around
	}

	public override void SetUpAction()
	{
		stats = GetComponent<Statistics>();
		AddPrecondition("NearEnemies", false);
		AddPrecondition("HasCure", true);
		AddPrecondition("isCured", false);
		AddPrecondition("isInfected", true);
		AddEffect("Healed", true);
	}

	protected override void ResetAction() //reset timer
	{
		startTime = 0;
		usedCure = false;
	}
}
