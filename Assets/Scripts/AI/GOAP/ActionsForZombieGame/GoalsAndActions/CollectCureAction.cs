using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCureAction : GoapAction
{
	HumanoidAgent hAgent;
	bool running = false;
	bool pickedUp = false;
	public override bool checkProceduralPrecondition(GoapAIAgent agent)
	{
		hAgent = (HumanoidAgent)agent;
		actionTarget = hAgent.sensor.ClosestObjectWithTag("Cure");
		return actionTarget != null;//if there is a cure in the level then we can run the action
	}

	public override bool IsCompleted()
	{
		return pickedUp; //items was picked up
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
			actionTarget = hAgent.sensor.ClosestObjectWithTag("Cure");
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
		if (inRange) //when in range collect it
		{
			hAgent.stats.inv.AddItem("Cure");
			Destroy(actionTarget);
			pickedUp = true;
		}
		return true;

	}

	public override void SetUpAction()
	{
		AddEffect("HasCure",true);
	}

	
	protected override void ResetAction()
	{
		pickedUp = false;
		running = false;
		inRange = false;
		actionTarget = null;
	}
}
