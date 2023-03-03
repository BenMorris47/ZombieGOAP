using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderAction : GoapAction
{
	bool ideled = false;
	public int extraWanderDistance = 5;
	public float wanderTimeOut = 5;
	float startTime = 0;
	HumanoidAgent hAgent;
	public override bool checkProceduralPrecondition(GoapAIAgent agent)
	{
		hAgent = (HumanoidAgent)agent;
		actionTarget = agent.waypoint.gameObject; //target is set to the waypoint
		agent.waypoint.position = agent.sensor.RandomPointOnNavMesh(hAgent.stats.viewRadius + extraWanderDistance); //move the waypoint to a random pos
		return true;//can always be performed
	}

	public override bool IsCompleted()
	{
		return ideled;
	}

	public override bool RequiresInRange()
	{
		return true;
	}

	public override bool RunAction()
	{
		if (hAgent.stats.AreEnemiesNear())//if enemies are near abort wander
		{
			return false;
		}
		if (inRange)//once at target complete
		{
			ideled = true;
		}
		if (startTime == 0) //set start time
		{
			startTime = Time.time;
		}
		else
		{
			if (Time.time - startTime > wanderTimeOut) //has wander time out been reached
			{
				return false;
			}
		}
		return true;
	}

	public override void SetUpAction()
	{
		AddEffect("Idled", true);
	}

	protected override void ResetAction()
	{
		ideled = false;
		inRange = false;
	}
}
