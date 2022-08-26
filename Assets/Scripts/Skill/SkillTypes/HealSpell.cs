using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSpell : Spell
{
	public override SpellType SpellType => SpellType.Heal;
	public override void CastSpell(Vector3 center, float spellRadius, int damage)
	{
		CastHeal(center, spellRadius, damage);
	}
	private void CastHeal(Vector3 center, float spellRadius, int damage)
	{
	}
}
