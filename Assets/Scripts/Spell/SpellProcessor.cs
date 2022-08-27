using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class SpellProcessor
{
	private static Dictionary<SpellType, Spell> _spells = new Dictionary<SpellType, Spell>();
	private static bool _initialized;

	private static void Initialize()
	{
		_spells.Clear();

		var assembly = Assembly.GetAssembly(typeof(Spell));

		var allSpellTypes = assembly.GetTypes()
			.Where(t => typeof(Spell).IsAssignableFrom(t) && t.IsAbstract == false);

		foreach (var spellType in allSpellTypes)
		{
			Spell spell = Activator.CreateInstance(spellType) as Spell;
			_spells.Add(spell.SpellType, spell);
		}
	}

	public static void CastSpell(SpellData spellData, Transform caster, Vector3 center)
	{
		if (_initialized == false)
			Initialize();

		var spell = _spells[spellData.Type];
		spell.CastSpell(spellData, caster, center);
	}
}
