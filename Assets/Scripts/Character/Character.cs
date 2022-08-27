using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, IStats
{
	[field: SerializeField] public float Health { get; set; }
	[field: SerializeField] public float Mana { get; set; }
	[field: SerializeField] public float Stamina { get; set; }

	public void GetDamage(float damage)
	{
		Health -= damage;
	}
}
