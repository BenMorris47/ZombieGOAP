using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Idamageable
{
    public void Die();//when the player dies
    public void TakeDamage(float damage);//when the player takes damage
    public void Heal(float healAmount);//when the player heals
    float GetCurrentHealth();//gets the current health
    float GetMaxHealth();//gets the max health
}
