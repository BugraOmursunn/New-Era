using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSkill : Skill
{
	public override SkillType SkillType => SkillType.Heal;
	public override void CastSkill(Vector3 center, float skillRadius, int damage)
	{
		CastHeal(center, skillRadius, damage);
	}
	private void CastHeal(Vector3 center, float skillRadius, int damage)
	{
	}
}
