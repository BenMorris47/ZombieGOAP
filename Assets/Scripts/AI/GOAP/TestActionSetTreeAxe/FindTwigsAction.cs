using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindTwigsAction : GoapAction
{
    bool collectedTwigs = false;

    public override bool checkProceduralPrecondition(GoapAIAgent agent)
    {
        actionTarget = GameObject.FindGameObjectWithTag("TwigPile");
        if (actionTarget != null)
        {
            return true;
        }
        return false;
    }

    public override bool IsCompleted()
    {
        return collectedTwigs;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    protected override void ResetAction()
    {
        inRange = false;
        actionTarget = null;
        collectedTwigs = false;
    }

    public override bool RunAction()
    {
        collectedTwigs = true;
        return true;
    }

    public override void SetUpAction()
    {
        effects.Add("hasWood", true);
        actionBaseCost = 20;
    }
}
