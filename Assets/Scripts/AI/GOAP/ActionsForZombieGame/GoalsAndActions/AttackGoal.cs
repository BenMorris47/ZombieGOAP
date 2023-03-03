using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackGoal : GoapGoal
{
    HumanoidAgent hAgent;
    public override bool CanPerformGoal(GoapAIAgent agent)
    {
        hAgent = (HumanoidAgent)agent;
        return hAgent.stats.AreEnemiesNear();//are enemies near
    }

    public override void CheckProciduralStates(GoapAIAgent agent)
    {
        
    }

    public override float GetCost(GoapAIAgent agent)
    {
        return goalBaseCost - (hAgent.stats.GetCurrentHealth()/10);//as health goes down the cost of attacks go up
    }

    public override void SetUpGoal()
    {
        AddGoalState("Attacked", true);
    }
}
