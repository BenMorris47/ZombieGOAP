using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChopTreeAction : GoapAction
{
    bool treeChopped = false;
    public float checkRange = 10;
    float startTime = 0;
    float chopDuration = 5;

    public override bool checkProceduralPrecondition(GoapAIAgent agent)
    {
        var colliders = Physics.OverlapSphere(agent.transform.position, checkRange);
        foreach (var item in colliders)
        {
            if (item.tag == "Tree")
            {
                actionTarget = item.gameObject;
                return true;
            }
        }
        return false;
    }

    public override bool IsCompleted()
    {
        return treeChopped;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    protected override void ResetAction()
    {
        treeChopped = false;
        actionTarget = null;
        inRange = false;
    }

    public override bool RunAction()
    {
        if (startTime == 0)
        {
            startTime = Time.time;
        }
        if (actionTarget == null)
        {
            return false;
        }
        else
        {
            if (Time.time - startTime > chopDuration)
            {
                Destroy(actionTarget);
                treeChopped = true;
            }
            return true;
        }
    }

    public override void SetUpAction()
    {
        preConditions.Add("hasAxe", true);
        effects.Add("hasWood", true);
        actionBaseCost = 5;
    }
}
