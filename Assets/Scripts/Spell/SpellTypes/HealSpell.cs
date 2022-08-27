using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSpell : Spell
{
	public override SpellType SpellType => SpellType.Heal;
	public override void CastSpell(SpellData spellData, Transform caster, Vector3 center)
	{
		CastHeal(spellData, caster, center);
	}
	private void CastHeal(SpellData spellData, Transform caster, Vector3 center)
	{
	}
}
