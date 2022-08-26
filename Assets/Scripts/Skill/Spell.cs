using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : MonoBehaviour
{
	public abstract SpellType SpellType { get; }
	public abstract void CastSpell(Vector3 center, float spellRadius, int damage);
}
