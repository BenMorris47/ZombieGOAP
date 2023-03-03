using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseMedpackAction : GoapAction
{
	
	float startTime = 0;
	public int healAmount = 20;
	public float healDuration = 5;
	public bool usedMedPack = false;
	private Statistics stats;
	public override bool checkProceduralPrecondition(GoapAIAgent agent)
	{
		return stats!= null && !stats.isZombie;//can run if human
	}

	public override bool IsCompleted()
	{
		return usedMedPack;
	}

	public override float GetCost(GoapAIAgent agent)
	{
		return (base.GetCost(agent) - healAmount) + healDuration; //takes into account how much will be healed but also the time to do so
	}

	public override bool RequiresInRange()
	{
		return false;
	}

	public override bool RunAction()
	{
		if (startTime == 0)//set start time
		{
			startTime = Time.time;
		}
		else
		{
			if (Time.time - startTime > healDuration)//has the healing duration past then heal
			{
				usedMedPack = true;
				stats.Heal(healAmount);
				stats.inv.RemoveItem("Medpack");
			}
		}
		return !stats.AreEnemiesNear();//carries out action while no enemies are around
	}

	public override void SetUpAction()
	{
		stats = GetComponent<Statistics>();
		AddPrecondition("NearEnemies", false);
		AddPrecondition("HasMedpack", true);
		AddEffect("Healed", true);
	}

	protected override void ResetAction()
	{
		startTime = 0;
		usedMedPack = false;
	}
}
