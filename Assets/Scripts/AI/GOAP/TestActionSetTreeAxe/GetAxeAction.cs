using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAxeAction : GoapAction
{
    bool axeCollected = false;
    public float checkRange = 5;
    public override bool checkProceduralPrecondition(GoapAIAgent agent)
    {
        var colliders = Physics.OverlapSphere(agent.transform.position, checkRange);
        foreach (var item in colliders)
        {
            if (item.tag == "Axe")
            {
                actionTarget = item.gameObject;
                return true;
            }
        }
        return false;
    }

    public override bool IsCompleted()
    {
        return axeCollected;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    protected override void ResetAction()
    {
        inRange = false;
        actionTarget = null;
        axeCollected = false;
    }

    public override bool RunAction()
    {
        if (actionTarget == null)
        {
            return false;
        }
        else
        {
            Destroy(actionTarget);
            axeCollected = true;
            return true;
        }
    }

    public override void SetUpAction()
    {
        effects.Add("hasAxe", true);
        actionBaseCost = 5;
    }
}
