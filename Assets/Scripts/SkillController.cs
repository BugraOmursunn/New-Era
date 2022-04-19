using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
	public GameObject[] prefabs;

	private void OnEnable()
	{
		InputEventManager.CastSkill += CastSkill;
	}
	private void OnDisable()
	{
		InputEventManager.CastSkill -= CastSkill;
	}
	private void CastSkill(SkillData skillType)
	{
		var skill = Instantiate(prefabs[skillType.skillIndex]);

		skill.transform.position = new Vector3(this.transform.position.x, skill.transform.position.y, this.transform.position.z);
		//skill.transform.parent = this.transform;
		skill.transform.rotation = Quaternion.Euler(0, this.transform.eulerAngles.y - 90, 0);
		Destroy(skill, 3f);
	}
}
