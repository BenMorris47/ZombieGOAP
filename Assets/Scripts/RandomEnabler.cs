using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEnabler : MonoBehaviour
{
	[Range(0,1)]
	public float chanceToSpawn = 0.3f;
	private void Start() //chooses randomly to enable the object
	{
		if (Random.Range(0f,1f) <= chanceToSpawn)
		{
			gameObject.SetActive(true);
		}
		else
		{
			gameObject.SetActive(false);
		}
	}
}
