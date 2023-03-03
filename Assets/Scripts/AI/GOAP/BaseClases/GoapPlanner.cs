using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public class GoapPlanner
{
    public bool useRandomResolver;//will plans of the same cost be chosen randomly?
    //creates and returns the plan
    public Queue<GoapAction> GeneratePlan(GoapAIAgent agent, HashSet<GoapAction> allAvailableActions, Dictionary<string, bool> currentWorldState, Dictionary<string, bool> goalState)
    {

        foreach (var action in allAvailableActions) //reset the actions
        {
            action.ResetActionValues();
        }

        //Create a list of actions that are actually possible ie if there are no axes in the world there is no point in checking a find axe action later
        HashSet<GoapAction> usableActions = new HashSet<GoapAction>();
        foreach (GoapAction action in allAvailableActions)
        {
            if (action.checkProceduralPrecondition(agent))
                usableActions.Add(action);
        }

        //go through every path to determine the nodes that reach the goal
        List<ActionNode> availablePaths = new List<ActionNode>();
        ActionNode startNode = new ActionNode(null, 0, null, currentWorldState); //create the starting node
        bool foundPath = GeneratePaths(startNode, availablePaths, usableActions, goalState, agent); //create a plan

        if (!foundPath)
        {
            Debug.Log("<color=red>No Plan Found</color>");
            return null;
        }
        else
        {
            Debug.Log("<color=green>Plan Found</color>");
        }

        //Find the cheapest path/node
        ActionNode cheapestNode = null;
        foreach (var node in availablePaths)
        {
            if (cheapestNode == null || node.pathCost < cheapestNode.pathCost)
            {
                cheapestNode = node;
            }
			if (node.pathCost == cheapestNode.pathCost && node != cheapestNode && useRandomResolver) //if the cost is the same and useRandomResolver then one is chosen at random
            {
				cheapestNode = Random.Range(0.0f, 1.0f) < 0.5f ? cheapestNode : node;
			}
		}

        //create a list for the queue of all nodes from their parents (have to create a list as doing this in queue and attempting to reverse it doesn't work)
        List<GoapAction> nodesForQueue = new List<GoapAction>();
        ActionNode currentNode = cheapestNode;
        while (currentNode != null)
        {
            if (currentNode.parentNode != null)
            {
                nodesForQueue.Insert(0,currentNode.action); //using insert as it is slightly more proformant than building a list then reversing it
            }
            currentNode = currentNode.parentNode;
        }

        //Create the queue of actions build from the nodes parents
        Queue<GoapAction> queue = new Queue<GoapAction>();
        foreach (var action in nodesForQueue)
        {
            queue.Enqueue(action);
        }
        return queue;
    }

    //used to build the path
    private bool GeneratePaths(ActionNode parent, List<ActionNode> availablePaths, HashSet<GoapAction> availableActions, Dictionary<string, bool> goalState, GoapAIAgent agent)
    {
        bool foundPlan = false; //have any plans been found

        
        foreach (var action in availableActions)//Run through all actions available to build the paths starting at the current world state
        {
            //check that the effects of the previouse nodes + starting world state allow this nodes action preconditions to be met
            if (ContainsAllStateValues(action.preConditions, parent.currentNodeState))
            {
                Dictionary<string, bool> currentState = CombineStates(action.effects, parent.currentNodeState); //combine the parents state with the effects of this node

                ActionNode node = new ActionNode(parent,parent.pathCost+action.GetCost(agent),action,currentState); //creates a new node and use action.GetCost() to calculate the procidural cost
                if (ContainsAllStateValues(goalState, currentState))//is a valid plan
                {
                    availablePaths.Add(node);
                    foundPlan = true;
                }
                else //isn't a valid plan
                {

                    //removes the current action from the available actions (this is to avoid an infinate loop of nodes that require no preconditions and try to branch the path further
                    HashSet<GoapAction> newAvailableActions = new HashSet<GoapAction>(availableActions);
                    newAvailableActions.Remove(action);
                    if (GeneratePaths(node, availablePaths, newAvailableActions, goalState, agent))
                    {
                        foundPlan = true;
                    }
                }
            }
        }
        return foundPlan;
    }

    //Checks if all the desired states are the same in the current state
    public bool ContainsAllStateValues(Dictionary<string, bool> desiredState, Dictionary<string, bool> currentState)
    {
        foreach (var dState in desiredState)
        {
            if (currentState.TryGetValue(dState.Key, out bool val))
            {
                if (val != dState.Value)
                    return false;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    //Combine two states together adding new states and setting existing values to new values
    private Dictionary<string, bool> CombineStates(Dictionary<string, bool> newStateValues, Dictionary<string, bool> baseState)
    {
        Dictionary<string, bool> combinedState = new Dictionary<string, bool>(baseState); //clone the base state into a new dictionary
        
        foreach (var state in newStateValues) //loop through all new states, if they exist in the base state change the value if they don't add them
        {
            string key = state.Key;
            bool val = state.Value;
            if (combinedState.ContainsKey(state.Key))
            {
                combinedState.Remove(state.Key);
                combinedState.Add(key, val);
            }
            else
            {
                combinedState.Add(key, val);
            }
        }
        return combinedState;
    }
}

public class ActionNode
{
    public ActionNode parentNode;
    public float pathCost = 0;
    public GoapAction action;
    public Dictionary<string, bool> currentNodeState;

    //constructor for node class
    public ActionNode(ActionNode parentNode, float pathCost, GoapAction action, Dictionary<string, bool> currentNodeState)
    {
        this.parentNode = parentNode;
        this.pathCost = pathCost;
        this.action = action;
        this.currentNodeState = currentNodeState;
    }
}
