using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class WeaponInteractionController : MonoBehaviour
{
	[SerializeField] private WeaponData weaponData;
	private bool CanAttack;
	private float damageInterval = 0.2f;
	private float tempTime;

	public SerializedDictionary<IDamageAble, float> attackTarget = new SerializedDictionary<IDamageAble, float>();

	public void EnableCanDamage()
	{
		CanAttack = true;
		Invoke(nameof(DisableCanDamage), 1f);
	}
	private void DisableCanDamage()
	{
		CanAttack = false;
	}
	private void OnTriggerEnter(Collider other)
	{
		if (CanAttack == false)
			return;

		if (other.TryGetComponent(out IDamageAble damageAble) == false)
			return;


		if (damageAble != this.transform.GetComponentInParent<IDamageAble>())
		{
			if (attackTarget.ContainsKey(damageAble))
				return;

			attackTarget.Add(damageAble, 1);
			int randomDmg = UnityEngine.Random.Range((int)weaponData.weaponModifierSettings.attackDamage - 20, (int)weaponData.weaponModifierSettings.attackDamage + 20);
			damageAble.GetDamage(-randomDmg);
		}
	}
	private void Update()
	{
		for (int i = 0; i < attackTarget.Count; i++)
		{
			attackTarget[attackTarget.ElementAt(i).Key] = attackTarget.ElementAt(i).Value - Time.deltaTime;

			if (attackTarget.ElementAt(i).Value <= 0)
			{
				attackTarget.Remove(attackTarget.ElementAt(i).Key);
			}
		}
	}
}
