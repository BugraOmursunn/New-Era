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
		if (InputEventManager.IsDrawSword2h() == false) return;

		animator.SetTrigger(skillData.skillAnimationTriggerName);
		InputEventManager.EnableWeaponTrail.Invoke();
		StartCoroutine(Cast(skillData));
	}
	private IEnumerator Cast(SkillData skillData)
	{
		yield return new WaitForSeconds(skillData.vfxActivationTime);

		var skill = Instantiate(skillData.skillPrefab);
		skill.transform.parent = this.transform;

		skill.transform.localPosition = new Vector3(0, skill.transform.position.y, 0) + skillData.offSet;

		skill.transform.rotation = Quaternion.Euler(0, this.transform.eulerAngles.y - 90, 0);

		if (skillData.isChild == false)
			skill.transform.parent = null;
		Destroy(skill, 3f);
	}
}
