using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : MonoBehaviour
{
	public abstract SpellType SpellType { get; }
	public abstract void CastSpell(SpellData spellData, Transform caster, Vector3 center);
}


// public abstract class Spell2<T>
// {
// 	public void TestSpell(T spell) { }
// }
