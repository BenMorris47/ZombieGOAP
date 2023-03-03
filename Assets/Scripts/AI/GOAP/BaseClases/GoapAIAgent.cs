using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class GoapAIAgent : MonoBehaviour
{
    [Header("Debugging settings")]
    public bool seeDebugLog = true;
    public bool isPaused = false;
    public bool useDefaultBackUpGoal;
    public TextMeshPro goalText;
    public TextMeshPro actionText;
    [Space]
    [Header("PlanInfo")]
    [SerializeField] private string currentGoal = string.Empty;
    [SerializeField] private string currentplanString;
    [Space]
    [Header("Ai Componants")]
    private NavMeshAgent agent;
    public Sensor sensor;
    public Transform waypoint;
    
    //Planning, Nodes, outcome and Important planning componants
    public HashSet<GoapAction> actions = new HashSet<GoapAction>();
    public Queue<GoapAction> currentPlan = new Queue<GoapAction>();
    public GoapPlanner planner;
    public GoapWorldStates worldState;
    public GoapGoalManager goalManager;
    public bool useRandomResolver = true;

    public float agentRangeOffsetMultiplyer = 1; //the multiplyer used on the action ranges
    
    //State Machine
    public enum AiState { Idle,Move,Act}
    public AiState currentState = AiState.Idle;

	private void Awake() //creates planner and sets if it should use random resolver
	{
        planner = new GoapPlanner();
        planner.useRandomResolver = useRandomResolver;
    }
	void Start() //Sets up the agents waypoint as well as the actions
    {
        GameObject tempGO = new GameObject("Waypoint");
        tempGO.transform.position = transform.position;
        waypoint = tempGO.transform;
        agent = GetComponent<NavMeshAgent>();
        actions = SetUpActions();

    }
    private void Update() //this runs the states machine that controls the ai behaviour
    {
		if (isPaused)//if paused returns to planning state an removes path
		{
            currentState = AiState.Idle;
            agent.SetDestination(transform.position);
            return;
		}
        switch (currentState)
        {
            case AiState.Idle:
                AIIdle();
                break;
            case AiState.Move:
                AIMove();
                break;
            case AiState.Act:
                AIAct();
                break;
            default:
                break;
        }
    }

    public bool hasPlan() { return currentPlan.Count > 0; }//Returns true if there is a plan

    private void AIAct() //This carries out the next action available
    {
        RunAction();
    }

    private void AIMove() //this moves the ai
    {
        MoveToAction();
    }

    private void AIIdle()//this is the planning phase for the ai
    {
        FindPlan();
    }

    private void FindPlan() //creating the plans
    {
        GoapGoal chosenGoal = goalManager.GetGoal(this); //First it gets a goal based on goal costs
        if (chosenGoal == null) //if there is no goal retuen
        {
            Log($"<color=red>NO GOAL FOUND</color>");
            return;
        }
        bool planningResult = GeneratePlanForGoal(chosenGoal);//Creates a plan and returns a bool if it is succesful
        if (!planningResult && useDefaultBackUpGoal)//if no plan is found and backup goal is allowed reevaluate with defult goal
		{
            chosenGoal = goalManager.defaultGoal; //set default goal
            Log("Falling Back To Default Goal");
            planningResult = GeneratePlanForGoal(chosenGoal);//regren plan
        }
        //fill out info for user and debugging purporse 
        currentGoal = chosenGoal.ToString();
        goalText.text = currentGoal;
        //set the state depending on if a plan exists
        currentState = planningResult ? AiState.Act : AiState.Idle;
    }

    private bool GeneratePlanForGoal(GoapGoal goal)//Plan Generation
	{
        //Plan Generator taking in all actions the current worlds state an the state that the goal requires
        var plan = planner.GeneratePlan(this, actions, worldState.GetCurrentWorldState(), goal.GetGoal(this));

        //mainly just strings for debugging no need to have in code
        currentplanString = $"<color=yellow>{gameObject.name}</color> <color=purple>PLAN: ";
        currentplanString += "Start";
        if (plan != null) //HAS PLAN
        {
            foreach (var item in plan)
            {
                currentplanString = $"{currentplanString} => <color=orange>{item}</color> ";
            }
            currentplanString += $"=> <color=blue>{goal}</color></color>";
            currentPlan = plan;
        }
        else //NO PLAN
        {
            currentplanString = $"<color=red>NO PLAN FOUND FOR GOAL</color>: <color=blue>{goal}</color>";
        }
        Log(currentplanString);//logs the entire plan
        return plan != null; //true if there is a plan
    }

    private void RunAction()//perform action
	{
        if (!hasPlan()) //if there is no plan return to planning
        {
            currentState = AiState.Idle;
            return;
        }

        GoapAction action = currentPlan.Peek(); //get current action
        if (action.IsCompleted()) //if action is conpleted remove it from the queue
        {
            currentPlan.Dequeue();
            Log($"<color=yellow>{gameObject.name}</color> <color=green> Completed action </color>: <color=orange>{action}</color>");
        }
        if (hasPlan())//if there are still action in the plan
        {
            action = currentPlan.Peek();//get current action
            bool inRange = action.RequiresInRange() ? action.inRange : true;//checks if actions needs to be in range and if so if it is already in range
            if (inRange) //In range
            {
                actionText.text = $"Performing {action}";
                if (!action.RunAction()) //Did the action run this frame successfully if not abort the plan and replan
                {
                    actionText.text = $"Failed {action}";
                    currentState = AiState.Idle;//return to planning
                    Abort($"<color=orange>{action}</color> did not run succesfully");
                }

            }
            else //Not in range and need to move
            {
                currentState = AiState.Move;
                Log($"<color=yellow>{gameObject.name}</color> Moving for <color=orange>{action}</color>");
            }
        }
        else //Completed all actions in the plan
        {
            actionText.text = $"Waiting For Plan";
            FinishedPlan();
        }
    }

    private void MoveToAction()//move to target before acting
	{
        GoapAction action = currentPlan.Peek();//gets the current action
        if (action.RequiresInRange() && action.actionTarget == null)//if there is no target and needs to be in range then abort
        {
            Abort($"Need to assign a target to <color=orange>{currentPlan.Peek()}</color>");
            return;
        }

        MoveTo(action.actionTarget.transform.position); //move towards the target
		if (agent.pathStatus == NavMeshPathStatus.PathPartial) //if the target can't be reached then abort
		{
            Abort($"<color=orange>{action}</color> couldn't reach target");
		}
        if (Vector3.Distance(transform.position, action.actionTarget.transform.position) <= action.range * agentRangeOffsetMultiplyer) //is in range?
        {
            action.inRange = true;
        }

        if (action.inRange) //if target is reached then action can run
        {
            currentState = AiState.Act;
        }
        actionText.text = $"Moving to {action}";
    }

    public void FinishedPlan() //Completed all actions in plan
	{
        currentState = AiState.Idle;//return to planning
        Log($"<color=yellow>{gameObject.name}</color> <color=green> Finished the Plan </color>: {currentplanString}");
    }

    public void Abort() //returns to idle planning state while logging abort message
	{
        actionText.text = $"Waiting For Plan";
        Log($"<color=yellow>{gameObject.name}</color> <color=red>Action Aborted:</color> Reason Unknown");
        currentState = AiState.Idle;
	}
    public void Abort(string abortReason)//Override: returns to idle planning state while logging abort message given with string
    {
        actionText.text = $"Waiting For Plan";
        Log($"<color=yellow>{gameObject.name}</color> <color=red>Action Aborted:</color> {abortReason}");
        currentState = AiState.Idle;
    }

    private void Log(string log) //Log is a debug.log that is used throughout allowing the logs to be toggled on and off
	{
		if (seeDebugLog)
		{
            Debug.Log(log);
        }
	}

    public HashSet<GoapAction> SetUpActions() //Generate a Hashset of all actions to prevent adding them multiple times
    {
        HashSet<GoapAction> actionsHashSet = new HashSet<GoapAction>();
        var actionsArray = GetComponents<GoapAction>();
        foreach (GoapAction action in actionsArray)
        {
            actionsHashSet.Add(action);
        }
        return actionsHashSet;
    }
    

    public void MoveTo(Vector3 position) //sets the destination of the NavMesh agent
    {
        agent.SetDestination(position);
        agent.isStopped = false;
    }

    public void AgentPaused(bool isPausedValue) //(un)Pauses agent 
	{
        Abort("Agent Was Paused");
        isPaused = isPausedValue;
        agent.isStopped = isPausedValue;
    }
	public virtual void OnDrawGizmosSelected() 
	{
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);//shows the forward direction of the ai
    }
}
