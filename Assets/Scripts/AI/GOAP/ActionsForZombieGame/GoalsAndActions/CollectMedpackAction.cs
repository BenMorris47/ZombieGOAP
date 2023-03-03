using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectMedpackAction : GoapAction
{
	HumanoidAgent hAgent;
	bool running = false;
	bool pickedUp = false;
	public override bool checkProceduralPrecondition(GoapAIAgent agent)
	{
		hAgent = (HumanoidAgent)agent;
		actionTarget = hAgent.sensor.ClosestObjectWithTag("Medpack"); //set closest medpack in the world
		return actionTarget != null;//was the target set
	}

	public override bool IsCompleted()
	{
		return pickedUp;
	}

	public override bool RequiresInRange()
	{
		return true;
	}

	public override bool RunAction()
	{
		//set the action target the first time it is ran so that it can find the closest one to its new pos
		if (actionTarget == null && !running)
		{
			actionTarget = hAgent.sensor.ClosestObjectWithTag("Medpack");
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
		if (inRange) //if in range pick up
		{
			hAgent.stats.inv.AddItem("Medpack");
			Destroy(actionTarget);
			pickedUp = true;
		}
		return true;

	}

	public override void SetUpAction()
	{
		AddEffect("HasMedpack",true);
		AddPrecondition("NearEnemies", false);
	}

	protected override void ResetAction()
	{
		pickedUp = false;
		running = false;
		inRange = false;
		actionTarget = null;
	}
}
