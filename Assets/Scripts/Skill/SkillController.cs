using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SkillController : MonoBehaviour
{
	public Animator playerAnimator;
	private Transform playerTransform;

	private GameTypes gameType;

	private void OnEnable()
	{
		gameType = EventManager.gameType.Invoke();
		if (gameType == GameTypes.MultiPlayer)
		{
			if (this.transform.parent.GetComponent<PhotonView>().IsMine == false)
				return;
		}

		InputEventManager.CastSkill += CastSkill;
	}
	private void OnDisable()
	{
		if (gameType == GameTypes.MultiPlayer)
		{
			if (this.transform.parent.GetComponent<PhotonView>().IsMine == false)
				return;
		}

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

		GameObject skill;
		Vector3 position = new Vector3(playerTransform.position.x, skillData.skillPrefab.transform.position.y, playerTransform.position.z);
		Quaternion transformRotation = Quaternion.Euler(0, playerTransform.eulerAngles.y - 90, 0);

		switch (gameType)
		{
			case GameTypes.SinglePlayer:
				skill = Instantiate(skillData.skillPrefab, position, transformRotation);
				break;
			case GameTypes.MultiPlayer:
				skill = PhotonNetwork.Instantiate(skillData.skillPrefab.name, position, transformRotation);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		skill.transform.parent = playerTransform;
		skill.transform.localPosition = new Vector3(0, skill.transform.position.y, 0) + skillData.offSet;
		skill.transform.rotation = transformRotation;

		if (skillData.isChild == false)
			skill.transform.parent = null;

		
		Destroy(skill, 3f);
	}
}
