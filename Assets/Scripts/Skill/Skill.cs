using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
	public abstract SkillType SkillType { get; }
	public abstract void CastSkill(Vector3 center, float skillRadius, int damage);
}
