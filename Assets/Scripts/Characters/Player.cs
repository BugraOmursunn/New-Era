using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
	public override float Health { get; set; }
	public override float Mana { get; set; }
	public override float Stamina { get; set; }
	public override bool IsDead { get; set; }

	private void Start()
	{
	}

	public override void GetDamage(float damage)
	{
	}
}
