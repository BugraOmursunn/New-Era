using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSpell : Spell
{
	public override SpellType SpellType => SpellType.Heal;
	public override void CastSpell(Transform caster, Vector3 center, float spellRadius, int damage)
	{
		CastHeal(caster, center, spellRadius, damage);
	}
	private void CastHeal(Transform caster, Vector3 center, float spellRadius, int damage)
	{
	}
}
