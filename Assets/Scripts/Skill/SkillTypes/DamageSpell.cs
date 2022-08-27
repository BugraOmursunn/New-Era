using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageSpell : Spell
{
	public override SpellType SpellType => SpellType.Damage;

	public override void CastSpell(Transform caster, Vector3 center, float spellRadius, int damage)
	{
		CastAttack(caster, center, spellRadius, damage);
	}

	private void CastAttack(Transform caster, Vector3 center, float spellRadius, int damage)
	{
		List<Collider> effectedCollider = Physics.OverlapSphere(center, spellRadius).ToList();

		// for (int i = 0; i < effectedCollider.Count; i++)
		// 	Debug.Log(effectedCollider[i].name);

		//select effected characters from effected colliders except original caster
		List<Character> effectedCharacters = new List<Character>();
		effectedCharacters = effectedCollider
			.Select(r => r.GetComponent<Character>())
			.Where(g => g != null)
			.Where(t => t.transform != caster.transform)
			.ToList();

		//Debug.Log(caster.name);
		// foreach (var item in effectedCollider)
		// {
		// 	item.TryGetComponent(out Character newChar);
		// 	if (newChar)
		// 		effectedCharacters.Add(newChar);
		// }

		for (int i = 0; i < effectedCharacters.Count; i++)
		{
			Debug.Log(effectedCharacters[i].name);
			effectedCharacters[i].GetDamage(5);
		}
	}
}
