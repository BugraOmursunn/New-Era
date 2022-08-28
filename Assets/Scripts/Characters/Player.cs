using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Player : Character
{
	[field: SerializeField] public override CharacterData CharData { get; set; }
	[field: SerializeField] public override Animator CharAnimator { get; set; }

	[field: SerializeField] public override float Health { get; set; }
	[field: SerializeField] public override float Mana { get; set; }
	[field: SerializeField] public override float Stamina { get; set; }

	[field: SerializeField] public override bool IsDead { get; set; }

	private void Start()
	{
	}

	public override void GetDamage(float damage)
	{
		if (IsDead == true)
			return;

		if (Health > 0 && IsDead == false)
		{
			Health += damage;

			if (Health <= 0)
			{
				IsDead = true;
			}

			CharAnimator.SetTrigger(Health > 0 ? CharData.GetHitAnim : CharData.DieAnim);
		}

		GameObject newDamageIndicator = ResourceManager.DamageIndicator(this.transform);
		newDamageIndicator.GetComponent<DamageIndicator>().Instantiate(damage);
	}

	private void OnValidate()
	{
		if (CharData == null)
			return;
		Health = CharData.Health;
		Mana = CharData.Mana;
		Stamina = CharData.Stamina;
	}
}
