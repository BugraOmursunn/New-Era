using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
	[SerializeField] private Animator playerAnimator;
	private Transform playerTransform;
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
		playerTransform = playerAnimator.transform;
	}
	private void CastSkill(SkillData skillData)
	{
		if (InputEventManager.IsDrawSword2h() == false) return;

		playerAnimator.SetTrigger(skillData.skillAnimationTriggerName);
		InputEventManager.EnableWeaponTrail.Invoke();
		StartCoroutine(Cast(skillData));
	}
	private IEnumerator Cast(SkillData skillData)
	{
		yield return new WaitForSeconds(skillData.vfxActivationTime);

		var skill = Instantiate(skillData.skillPrefab);
		skill.transform.parent = playerTransform;

		skill.transform.localPosition = new Vector3(0, skill.transform.position.y, 0) + skillData.offSet;

		skill.transform.rotation = Quaternion.Euler(0, playerTransform.eulerAngles.y - 90, 0);

		if (skillData.isChild == false)
			skill.transform.parent = null;
		
		Destroy(skill, 3f);
	}
}
