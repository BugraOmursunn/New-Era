using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastSkill : MonoBehaviour
{
	void Start()
	{
		AttackSkill newSkill = gameObject.AddComponent<AttackSkill>();
		newSkill.CastSkill(this.transform.position, 2, 2);
	}
}
