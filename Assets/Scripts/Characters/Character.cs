using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, IStats, IDamageAble
{
	public float Health { get; set; }
	public float Mana { get; set; }
	public float Stamina { get; set; }
	
	public bool IsDead { get; set; }
	
	public void GetDamage(float damage)
	{
		Health += damage;
	}
}
