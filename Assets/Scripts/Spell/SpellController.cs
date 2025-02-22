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
		{
			Debug.Log("I can't do that");
			return false;
		}

		var manaCost = spellData.spellModifierSettings.manaCost;
		
		if (EventManager.IsHaveEnoughMana.Invoke(manaCost) == false)
		{
			Debug.Log("Not enough mana");
			return false;
		}

		playerAnimator.SetTrigger(spellData.spellVFXSettings.AnimTriggerName);
		StartCoroutine(Cast(spellData));
		EventManager.DecreaseMana.Invoke(manaCost);
		InputEventManager.EnableWeaponTrail.Invoke();
		return true;
	}
	private IEnumerator Cast(SpellData spellData)
	{
		yield return new WaitForSeconds(spellData.spellVFXSettings.vfxActivationTime);

		Vector3 position = new Vector3(playerTransform.position.x, spellData.spellVFXSettings.Prefab.transform.position.y, playerTransform.position.z);
		Quaternion transformRotation = Quaternion.Euler(0, playerTransform.eulerAngles.y + spellData.spellVFXSettings.rotOffSet.y, 0);

		//for single, multi selection
		GameObject spell = GameTypePrefabManager.ReturnGameTypeSelectionPrefab(spellData.spellVFXSettings.Prefab, position, transformRotation);

		spell.transform.parent = playerTransform;
		spell.transform.localPosition = new Vector3(0, spell.transform.position.y, 0) + spellData.spellVFXSettings.posOffSet;
		spell.transform.rotation = transformRotation;

		//where the real magic happens
		SpellProcessor.CastSpell(spellData, playerTransform, spell.transform.position);

		if (spellData.spellVFXSettings.isChild == false)
			spell.transform.parent = null;

		StartCoroutine(Delay(spell));
	}
	private IEnumerator Delay(GameObject spell)
	{
		yield return new WaitForSeconds(5);

		if (PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.Destroy(spell);
		}
		else
		{
			Destroy(spell);
		}
	}
}
