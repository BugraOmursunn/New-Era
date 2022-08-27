using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpellController : MonoBehaviour
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

		InputEventManager.CastSpell += CastSpell;
	}
	private void OnDisable()
	{
		if (gameType == GameTypes.MultiPlayer)
		{
			if (this.transform.parent.GetComponent<PhotonView>().IsMine == false)
				return;
		}

		InputEventManager.CastSpell -= CastSpell;
	}
	private void Awake()
	{
		playerTransform = playerAnimator.transform;
	}
	private bool CastSpell(SpellData spellData)
	{
		if (InputEventManager.IsDrawSword2h() == false)
			return false;

		playerAnimator.SetTrigger(spellData.AnimTriggerName);
		InputEventManager.EnableWeaponTrail.Invoke();
		StartCoroutine(Cast(spellData));
		return true;
	}
	private IEnumerator Cast(SpellData spellData)
	{
		yield return new WaitForSeconds(spellData.vfxActivationTime);

		GameObject spell;
		Vector3 position = new Vector3(playerTransform.position.x, spellData.Prefab.transform.position.y, playerTransform.position.z);
		Quaternion transformRotation = Quaternion.Euler(0, playerTransform.eulerAngles.y - 90, 0);

		switch (gameType)
		{
			case GameTypes.SinglePlayer:
				spell = Instantiate(spellData.Prefab, position, transformRotation);
				break;
			case GameTypes.MultiPlayer:
				spell = PhotonNetwork.Instantiate(spellData.Prefab.name, position, transformRotation);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		spell.transform.parent = playerTransform;
		spell.transform.localPosition = new Vector3(0, spell.transform.position.y, 0) + spellData.offSet;
		spell.transform.rotation = transformRotation;

		//where the real magic happens
		SpellProcessor.CastSpell(spellData.Type, playerTransform, spell.transform.position, 2, 2);

		if (spellData.isChild == false)
			spell.transform.parent = null;

		Destroy(spell, 3f);
	}
}
