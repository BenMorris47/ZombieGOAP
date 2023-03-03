using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HumanoidAgent : GoapAIAgent
{
    
    public Statistics stats;//holds a refrence to the Statistics
	public override void OnDrawGizmosSelected() //used to visuallise the view distance
	{
		base.OnDrawGizmosSelected();
		Gizmos.DrawWireSphere(transform.position, stats.viewRadius);
	}
}
