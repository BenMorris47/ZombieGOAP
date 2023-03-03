using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ZombieGameManager : MonoBehaviour
{
    public static ZombieGameManager instance;
	public int zombieCount = 0;
	public int humanCount = 0;
	public TextMeshProUGUI counterText;
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		if (instance != this)
		{
			Destroy(this);
		}
	}

	private void Update()
	{
		counterText.text = $"Zombie Count: {zombieCount}\nHuman Count: {humanCount}";
	}

}
