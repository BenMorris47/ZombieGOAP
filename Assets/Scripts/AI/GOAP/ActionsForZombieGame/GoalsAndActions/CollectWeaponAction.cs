using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectWeaponAction : GoapAction
{
	HumanoidAgent hAgent;
	bool running = false;
	bool pickedUp = false;
	public override bool checkProceduralPrecondition(GoapAIAgent agent)
	{
		hAgent = (HumanoidAgent)agent;
		if (hAgent.stats.isZombie) //zombies can't collect weapons
		{
			return false;
		}
		actionTarget = hAgent.sensor.ClosestObjectWithTag("Weapon"); //is there a weapon close in the world
		return actionTarget != null; //has a weapon been found
	}

	public override bool IsCompleted()
	{
		return pickedUp;
	}

	public override bool RequiresInRange()
	{
		return true;//needs to be at the weapon
	}

	public override bool RunAction()
	{
		//set the action target the first time it is ran so that it can find the closest one to its new pos
		if (actionTarget == null && !running)
		{
			actionTarget = hAgent.sensor.ClosestObjectWithTag("Weapon");
			if (actionTarget == null)
			{
				return false;
			}
			running = true;
		}
		else if (actionTarget == null && running)
		{
			return false;
		}
		if (inRange) //when in range pick up weapon
		{
			hAgent.stats.inv.AddItem("Weapon");
			Destroy(actionTarget);
			pickedUp = true;
		}
		return true;

	}

	public override float GetCost(GoapAIAgent agent) //the cost depends on how close the weapon is so that if a battle starts with a weapon near by it is better to collect it
	{
		return Vector3.Distance(transform.position,actionTarget.transform.position);
	}

	public override void SetUpAction()
	{
		AddEffect("HasWeapon", true);
	}

	protected override void ResetAction()
	{
		pickedUp = false;
		running = false;
		inRange = false;
		actionTarget = null;
	}
}
