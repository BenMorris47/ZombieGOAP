using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sensor : MonoBehaviour
{
	[Range(-1,1)]
	public float fleeDotDirection = -0.25f;
    public GameObject ClosestObjectWithTag(string objTag) //returns the object closest to the agent
    {
		GameObject closestObject = null;
		float closestDistance = 0;
		foreach (var item in GameObject.FindGameObjectsWithTag(objTag)) //finds all objects
		{
			float distFromTag = Vector3.Distance(transform.position, item.transform.position);//get distance for comparing
			if (closestObject == null)//if there is no target auto set this as the closest
			{
				closestObject = item;
				closestDistance = distFromTag;
				continue;
			}
			if (closestDistance > distFromTag)//if this item is closer than the current closest then set to the closest
			{
				closestObject = item;
				closestDistance = distFromTag;
			}
		}
		return closestObject;
    }
	public GameObject ClosestObjectWithTagSphereCast(string objTag, float radius, bool canSeeWithRaycast)//returns the closest object in radius with tag and optional sight lines
	{
		GameObject closestObject = null;
		float closestDistance = 0;
		foreach (var item in Physics.OverlapSphere(transform.position, radius))//get all objects in radius
		{
			if (item.tag != objTag) //if tag is not a match skip
			{
				continue;
			}
			float distFromTag = Vector3.Distance(transform.position, item.transform.position);//get the distance
			if (closestObject == null) //set as the closest if closest is not chosen
			{
				closestObject = item.gameObject;
				closestDistance = distFromTag;
				continue;
			}
			if (closestDistance > distFromTag)//if the distance is closer than current closest 
			{
				if (canSeeWithRaycast && Physics.Raycast(transform.position, transform.position - item.transform.position,out RaycastHit hit))//raycast to try hit item
				{
					if (hit.collider == item)//if it is the same it has sightline, add as closest
					{
						closestObject = item.gameObject;
						closestDistance = distFromTag;
					}
				}
				else if (!canSeeWithRaycast)//if raycast not needed set as closest
				{
					closestObject = item.gameObject;
					closestDistance = distFromTag;
				}
			}
		}
		return closestObject;
	}

	public int AmountObjectWithTagSphereCast(string objTag, float radius, bool canSeeWithRaycast)//returns the amount of objects with a tag in a radius, can also raycast for lines of sight
	{
		int amountOfObjects = 0;
		foreach (var item in Physics.OverlapSphere(transform.position, radius))//find all items in radius
		{
			if (item.tag != objTag)//if the item tag doesn't match, skip
			{
				continue;
			}
			if (canSeeWithRaycast && Physics.Raycast(transform.position, item.transform.position - transform.position , out RaycastHit hit))//check line of sight if needed
			{
				if (hit.collider == item) //add item if its the right one
				{
					amountOfObjects++;
				}
			}
			else if(!canSeeWithRaycast) //add item if line of sight is not needed
			{
				amountOfObjects++;
			}
		}
		return amountOfObjects;
	}
	public int AmountObjectWithSphereCast(float radius, bool canSeeWithRaycast)//returns all objects within a radius, can optionally use a raycast to check line of sight
	{
		int amountOfObjects = 0;
		foreach (var item in Physics.OverlapSphere(transform.position, radius))
		{
			if (canSeeWithRaycast && Physics.Raycast(transform.position, transform.position - item.transform.position, out RaycastHit hit))//if raycast is needed check line of sight
			{
				if (hit.collider == item)
				{
					amountOfObjects++;
				}
			}
			else if (!canSeeWithRaycast)//add if raycast is not needed
			{
				amountOfObjects++;
			}
		}
		return amountOfObjects;
	}

	public int AmountObjectsWithTag(string objTag)//returns the length of objects with the given tag
	{
		return GameObject.FindGameObjectsWithTag(objTag).Length;
	}


	public Vector3 RandomPointOnNavMesh(float radius)//find random pos inside of UnitSphere with the given radius to sample on the navmesh (returns the closest navmesh point)
	{
		Vector3 randomDirection = Random.insideUnitSphere * radius;
		randomDirection += transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition(randomDirection, out hit, radius + 1, -1);
		return hit.position;
	}

	//samples a certain amount of random locations returning one withing a certain cone away from the agent
	public Vector3 TryFindFleeLocation(Vector3 fleeFromPos,float radius, int sampleAttempts) 
	{
		
		Vector3 fleePos = RandomPointOnNavMesh(radius); //Gets a random pos
		
		if (Vector3.Dot((transform.position - fleeFromPos).normalized, (transform.position - fleePos).normalized) < fleeDotDirection) //is the position in the dot cone facing away from the enemy (Default .25f = 40 Degree Cone)
		{
			return fleePos;
		}
		return TryFindFleeLocation(fleeFromPos, radius, sampleAttempts, 1); //if no valid pos was found try again
	}

	//Override to allow amount of previouse attempts (essentially the same as above)
	private Vector3 TryFindFleeLocation(Vector3 fleeFromPos, float radius, int sampleAttempts, int previousAttempts) 
	{
		Vector3 fleePos = RandomPointOnNavMesh(radius);
		if (sampleAttempts >= previousAttempts) //if the sample limit is reached then return the agents pos to fail
		{
			return transform.position;
		}
		if (Vector3.Dot((transform.position - fleeFromPos).normalized, (transform.position - fleePos).normalized) < fleeDotDirection)
		{
			return fleePos;
		}
		return TryFindFleeLocation(fleeFromPos, radius, sampleAttempts, previousAttempts++);//recursivly call this function while iterating the previouse attempts
	}
}
