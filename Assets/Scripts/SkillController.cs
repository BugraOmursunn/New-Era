using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
	private Animator animator;
	private void OnEnable()
	{
		InputEventManager.CastSkill += CastSkill;
	}
	private void OnDisable()
	{
		InputEventManager.CastSkill -= CastSkill;
	}
	private void Awake()
	{
		animator = this.GetComponent<Animator>();
	}
	private void CastSkill(SkillData skillData)
	{
		switch (skillData.skillName)
		{
			case SkillName.Windfury:
				if (InputEventManager.IsDrawSword2h() == false) return;

				animator.SetTrigger("SwordSpin");
				InputEventManager.EnableWeaponTrail.Invoke(3);
				StartCoroutine(Cast(skillData));
				break;
			case SkillName.SuperSlash:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
	private IEnumerator Cast(SkillData skillData)
	{
		yield return new WaitForSeconds(skillData.activationTime);

		var skill = Instantiate(skillData.skillPrefab);

		skill.transform.position = new Vector3(this.transform.position.x, skill.transform.position.y, this.transform.position.z);

		if (skillData.isChild == true)
			skill.transform.parent = this.transform;

		skill.transform.rotation = Quaternion.Euler(0, this.transform.eulerAngles.y - 90, 0);

		Destroy(skill, 3f);
	}
}
