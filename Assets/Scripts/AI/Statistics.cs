using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Statistics : GoapWorldStates,Idamageable
{
    public BasicInventory inv;
    [SerializeField] private int maxHealth = 10;
    [SerializeField]private float currentHealth;
    public bool isInfected = false;
    public bool isZombie = false;
    public bool isCured = false;
    public bool isAlive = true;
    public float viewRadius = 5;
    public float infectionDamageRate = 10f;
    public GoapAIAgent agent;
    
    public Material zombieMat;
    public Material humanMat;
    private void Start()//sets count for humans, sets health to max and sets agent
    {
        if (ZombieGameManager.instance != null)
        {
            ZombieGameManager.instance.humanCount++;
        }
        currentHealth = maxHealth;
        agent = GetComponent<GoapAIAgent>();
    }

	private void Update() //used to take damage when infected
	{
		if (isInfected && !isZombie && !isCured)
		{
            TakeDamage(infectionDamageRate * Time.deltaTime);
		}
		
	}
	public override Dictionary<string, bool> GetCurrentWorldState() //sets up and returns the current world state
    {
        var state = new Dictionary<string, bool>();
        state.Add("isAlive", currentHealth > 0);
        state.Add("isInfected", isInfected);
        state.Add("isCured", isCured);
        state.Add("isZombie", isZombie);
        state.Add("HasMedpack", inv.doesHaveItem("Medpack"));
        state.Add("HasCure", inv.doesHaveItem("Cure"));
        state.Add("HasWeapon", inv.doesHaveItem("Weapon"));
        state.Add("NearEnemies", AreEnemiesNear());
        return state;
    }

    public bool AreEnemiesNear() //returns true if there is a agent of the oposite type (zombie or human) near otherwise returns false
	{
        if (isZombie && agent.sensor.AmountObjectWithTagSphereCast("Human", viewRadius, true) > 0)
        {
            return true;
        }
        else if (!isZombie && agent.sensor.AmountObjectWithTagSphereCast("Zombie", viewRadius, true) > 0)
        {
            return true;
        }
		else
		{
            return false;
		}
    }

    public void TakeDamage(float damage) //takes damage, calls Die function if health goes below 0
    {
        currentHealth -= damage;
        if (currentHealth <= 0 && isAlive)
        {
            Die();
        }
        
    }
    public void Die() //Agent Dwies
    {
        agent.Abort("Died");//stops current tasks
        tag = "Dead";//sets the tag so that behaviours can use it correctly
        isAlive = false;
        //removes agent from the correct counter
        if (!isZombie)
        {
            if (ZombieGameManager.instance != null)
            {
                ZombieGameManager.instance.humanCount--;
            }
        }
        else
        {
            if (ZombieGameManager.instance != null)
            {
                ZombieGameManager.instance.zombieCount--;
            }
            
        }

        if (isInfected && !isCured)//if infected and human then begin turning into zombie
        {
            Turn();
        }
        agent.AgentPaused(true);//stops all new plans from calculating as well as stopping the nav mesh agent from moving
    }

    public async void Turn() //converts humans to zombies and vise versa
    {
        await Task.Delay(3000);//wait 3 seconds
        isAlive = true;
        if (isInfected && !isCured)
        {
            tag = "Zombie";//tags used to direct behaviours
            gameObject.name = "AiZombie"; //renames to help identify in logs what sort of agent this is
            GetComponent<MeshRenderer>().material = zombieMat;//sets mat to help identify
            isZombie = true;
			if (ZombieGameManager.instance != null) //adds count back to the corrcect side
			{
                ZombieGameManager.instance.zombieCount++;
            }
        }
        else if(isInfected && isCured) //same function as above only with humans
        {
            tag = "Human";
            GetComponent<MeshRenderer>().material = humanMat;
            gameObject.name = "AiHuman";
            isZombie = false;
            if (ZombieGameManager.instance != null)
            {
                ZombieGameManager.instance.humanCount++;
            }
        }
        currentHealth = maxHealth; //resets health to full
        agent.AgentPaused(false);//unpauses the agent
    }
    public void Heal(float healAmount) //adds health and makes sure the current health doesn't go over the max health
    {
        currentHealth += healAmount;
        currentHealth = currentHealth > maxHealth ? maxHealth : currentHealth;
    }

    public void Infect() //used to infect an agent
    {
		if (!isInfected)
		{
            isInfected = true;
            gameObject.name = gameObject.name + " (INFECTED)"; //add infected to name to see whos infected
        }
    }

    public void Cure()//used to cure and protect a person from infection
	{
        isCured = true;
        gameObject.name = "Human";
        if (ZombieGameManager.instance != null)
        {
            ZombieGameManager.instance.zombieCount--;//remove a zombie from the world
        }
        Turn();

    }
    private void OnDrawGizmos() //used to show info about infections and alive stats
    {
        Gizmos.color = isZombie ? Color.green : Color.red;//zombie = green human = red
        Gizmos.color = isAlive ? Gizmos.color : Color.blue;//if dead override colours to blue
        Gizmos.DrawSphere(transform.position, 0.75f);
        Gizmos.color = isInfected ? Color.cyan : Gizmos.color; //infected = cyan
        Gizmos.color = isCured ? Color.white : Gizmos.color;//cured = white
        Gizmos.DrawSphere(transform.position+ transform.up, 0.5f);
        Gizmos.color = inv.doesHaveItem("Weapon") ? Color.yellow : Color.grey; //Weapon = yellow
        Gizmos.DrawSphere(transform.position + transform.forward + (transform.right * .5f), 0.25f);
        Gizmos.color = inv.doesHaveItem("Medpack") ? Color.magenta : Color.grey; //Medpack = magenta
        Gizmos.color = inv.doesHaveItem("Cure") ? Color.green : Gizmos.color;//cure = green
        Gizmos.DrawSphere(transform.position + transform.forward - (transform.right * .5f), 0.25f);
        
    }

    public float GetCurrentHealth() //gets currentHealth
    {
        return currentHealth;
    }
    public float GetMaxHealth()//gets maxHealth
    {
        return maxHealth;
    }
}
