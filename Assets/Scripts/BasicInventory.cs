using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicInventory : MonoBehaviour
{
    public Dictionary<string, int> inv = new Dictionary<string, int>(); //inventory

	//checks that the item is in the inventory
    public bool doesHaveItem(string item)
	{
		if (inv.ContainsKey(item))
		{
			if (inv[item] > 0)
			{
				return true;
			}
		}
		return false;
	}
	//Override: checks that the item is in the inventory and gives the amount as an out value
	public bool doesHaveItem(string item, out int amount)
	{
		amount = 0;
		if (inv.ContainsKey(item))
		{
			if (inv[item] > 0)
			{
				amount = inv[item];
				return true;
			}
		}
		return false;
	}

	//Adds one item using a string as a key
	public void AddItem(string item)
	{
		if (inv.ContainsKey(item))
		{
			inv[item]++;
		}
		else
		{
			inv.Add(item, 1);
		}
	}
	//Override: Adds multiple items using an amount and a string as a key
	public void AddItem(string item, int amount)
	{
		if (inv.ContainsKey(item))
		{
			inv[item]+=amount;
		}
		else
		{
			inv.Add(item, amount);
		}
	}

	//Removes one item using a string as the key
	public void RemoveItem(string item)
	{
		if (inv.ContainsKey(item))
		{
			inv[item]--;
			if (inv[item] <= 0)
			{
				inv.Remove(item);
			}
		}
		
	}
	//Override: Removes multiple items using an amount and a string as the key
	public void RemoveItem(string item, int amount)
	{
		if (inv.ContainsKey(item))
		{
			inv[item] -= amount;
			if (inv[item] <= 0)
			{
				inv.Remove(item);
			}
		}
	}

	//Transfers one item from one invetory to another using a string as a key
	public bool TransferItem(string item, BasicInventory fromInventory, BasicInventory toInventory)
	{
		if (fromInventory.doesHaveItem(item))
		{
			fromInventory.RemoveItem(item);
			toInventory.AddItem(item);
			return true;
		}
		return false;
	}
	//Override: Transfers multiple items from one invetory to another using a string as a key
	public bool TransferItem(string item, int amount, BasicInventory fromInventory, BasicInventory toInventory)
	{
		if (fromInventory.doesHaveItem(item, out int amountInStorage) && amountInStorage >= amount)
		{
			fromInventory.RemoveItem(item,amount);
			toInventory.AddItem(item,amount);
			return true;
		}
		return false;
	}
}
