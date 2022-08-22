using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SkillController : MonoBehaviour
{
	public Animator playerAnimator;
	private Transform playerTransform;
	private void OnEnable()
	{
		if (this.transform.parent.GetComponent<PhotonView>().IsMine == false)
			return;

		InputEventManager.CastSkill += CastSkill;
	}
	private void OnDisable()
	{
		if (this.transform.parent.GetComponent<PhotonView>().IsMine == false)
			return;

		InputEventManager.CastSkill -= CastSkill;
	}
	private void Awake()
	{
		playerTransform = playerAnimator.transform;
	}
	private bool CastSkill(SkillData skillData)
	{
		if (InputEventManager.IsDrawSword2h() == false)
			return false;

		playerAnimator.SetTrigger(skillData.skillAnimationTriggerName);
		InputEventManager.EnableWeaponTrail.Invoke();
		StartCoroutine(Cast(skillData));
		return true;
	}
	private IEnumerator Cast(SkillData skillData)
	{
		yield return new WaitForSeconds(skillData.vfxActivationTime);

		var skill = PhotonNetwork.Instantiate(skillData.skillPrefab.name,
			new Vector3(playerTransform.position.x, skillData.skillPrefab.transform.position.y, playerTransform.position.z),
			Quaternion.Euler(0, playerTransform.eulerAngles.y - 90, 0));
		skill.transform.parent = playerTransform;

		skill.transform.localPosition = new Vector3(0, skill.transform.position.y, 0) + skillData.offSet;

		skill.transform.rotation = Quaternion.Euler(0, playerTransform.eulerAngles.y - 90, 0);

		if (skillData.isChild == false)
			skill.transform.parent = null;

		Destroy(skill, 3f);
	}
}
