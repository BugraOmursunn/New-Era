using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStats
{
	public float Health { get; set; }
	public float Mana { get; set; }
	public float Stamina { get; set; }

	public void GetDamage(float damage) { }
}
