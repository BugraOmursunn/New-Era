using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
	public GameObject[] prefabs;

	private void OnEnable()
	{
		InputEventManager.CastSkill1 += CastSkill;
	}
	private void OnDisable()
	{
		InputEventManager.CastSkill1 -= CastSkill;
	}
	private void CastSkill()
	{
		var skill = Instantiate(prefabs[0]);
		skill.transform.position = new Vector3(this.transform.position.x, skill.transform.position.y, this.transform.position.z);
		Destroy(skill, 3f);
	}
}
