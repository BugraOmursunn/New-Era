using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSpell : Spell
{
	public override SpellType SpellType => SpellType.Damage;

	public override void CastSpell(Vector3 center, float spellRadius, int damage)
	{
		CastAttack(center, spellRadius, damage);
	}

	private void CastAttack(Vector3 center, float spellRadius, int damage)
	{
		Collider[] effectedCollider = Physics.OverlapSphere(center, spellRadius);

		for (int i = 0; i < effectedCollider.Length; i++)
			Debug.Log(effectedCollider[i].name);
	}
}
