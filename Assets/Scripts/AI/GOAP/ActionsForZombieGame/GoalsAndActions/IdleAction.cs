using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAction : GoapAction
{
	bool ideled = false;
	float startTime = 0;
	public float idleDuration = 5;
	public Vector2 minMaxIdleDuration = new Vector2(0.5f, 3);
	HumanoidAgent hAgent;
	public override bool checkProceduralPrecondition(GoapAIAgent agent)
	{
		hAgent = (HumanoidAgent)agent;
		idleDuration = Random.Range(minMaxIdleDuration.x, minMaxIdleDuration.y);//set duration between two floats
		return true;
	}

	public override bool IsCompleted()
	{
		return ideled;
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
			if (Time.time - startTime > idleDuration)//has the idle duration completed
			{
				ideled = true;
			}
		}
		return !hAgent.stats.AreEnemiesNear(); //fail if enemies come near
	}

	public override void SetUpAction()
	{
		AddEffect("Idled", true);
	}

	protected override void ResetAction()
	{
		ideled = false;
		startTime = 0;
	}
}
