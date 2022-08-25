using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSkill : Skill
{
	public override void CastSkill(Vector3 center, float skillRadius, int damage)
	{
		CastAttack(center, skillRadius, damage);
	}

	private void CastAttack(Vector3 center, float skillRadius, int damage)
	{
		Collider[] effectedCollider = Physics.OverlapSphere(center, skillRadius);

		for (int i = 0; i < effectedCollider.Length; i++)
			Debug.Log(effectedCollider[i].name);
	}
}
