using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageSpell : Spell
{
	public override SpellType SpellType => SpellType.Damage;

	public override void CastSpell(SpellData spellData, Transform caster, Vector3 center)
	{
		CastAttack(spellData, caster, center);
	}

	private void CastAttack(SpellData spellData, Transform caster, Vector3 center)
	{
		List<Collider> effectedCollider = Physics.OverlapSphere(center, spellData.spellModifierSettings.spellRadius).ToList();

		// for (int i = 0; i < effectedCollider.Count; i++)
		// 	Debug.Log(effectedCollider[i].name);

		//select effected characters from effected colliders except original caster
		List<IDamageAble> effectedCharacters = new List<IDamageAble>();
		effectedCharacters = effectedCollider
			.Select(r => r.GetComponent<IDamageAble>())
			.Where(g => g != null)
			.Where(t => t != caster.GetComponent<IDamageAble>())
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
			Debug.Log(effectedCharacters[i]);
			effectedCharacters[i].GetDamage(spellData.spellModifierSettings.spellDamage);
		}
	}
}
