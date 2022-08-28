using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DummyTarget : MonoBehaviour, IStats, IDamageAble
{
	[field: SerializeField] public float Health { get; set; }
	[field: SerializeField] public float Mana { get; set; }
	[field: SerializeField] public float Stamina { get; set; }
	[field: SerializeField] public bool IsDead { get; set; }

	public Animator animator;

	public void GetDamage(float damage)
	{
		if (IsDead == true)
			return;

		if (Health > 0 && IsDead == false)
		{
			Health += damage;

			if (Health <= 0)
			{
				IsDead = true;
				Invoke(nameof(Resurrection), 3f);
			}

			animator.SetTrigger(Health > 0 ? "GetHit" : "Die");
		}

		var newDamageIndicator = Instantiate(ResourcesContainer.DamageIndicator(), this.transform.position, Quaternion.identity);
		newDamageIndicator.GetComponent<DamageIndicator>().Instantiate(damage);
	}
	private void Resurrection()
	{
		animator.SetTrigger("Resurrection");
		Health = 10;
		IsDead = false;
	}
}
