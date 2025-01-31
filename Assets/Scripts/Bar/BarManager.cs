using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class BarManager : MonoBehaviour
{
	[SerializeField] private SpellBook spellBook;

	[CanBeNull]
	private ScriptableObject m_BarItemData;

	[SerializeField] private bool isCasting;
	[SerializeField] private Transform[] buttons;
	[SerializeField] private List<BarCooldownData> barCooldownData;

	private void OnEnable()
	{
		GameTypes gameType = EventManager.gameType.Invoke();
		if (gameType == GameTypes.MultiPlayer)
		{
			if (this.transform.parent.GetComponent<PhotonView>().IsMine == false)
				return;
		}

		InputEventManager.BarIndexPressed += BarSpellPressed;
		InputEventManager.SetIsCasting += SetIsCasting;
		InputEventManager.IsCastingContinue += IsCastingContinue;
	}
	private void OnDisable()
	{
		GameTypes gameType = EventManager.gameType.Invoke();
		if (gameType == GameTypes.MultiPlayer)
		{
			if (this.transform.parent.GetComponent<PhotonView>().IsMine == false)
				return;
		}

		InputEventManager.BarIndexPressed -= BarSpellPressed;
		InputEventManager.SetIsCasting -= SetIsCasting;
		InputEventManager.IsCastingContinue -= IsCastingContinue;
	}
	private void Awake()
	{
		spellBook = EventManager.GetCharacterData.Invoke().SpellBook;
		//buttons = GameObject.FindObjectsOfType<BarItemManager>().Select(x => x.GetComponent<Transform>()).OrderBy(m => m.transform.GetSiblingIndex()).ToArray();
		//WeaponData weaponData = slotsData.barItems[i] as WeaponData;

		foreach (var scriptable in spellBook.barItems)
		{
			if (scriptable == null)
				continue;

			var newData = new BarCooldownData();

			switch (scriptable)
			{
				case SpellData spellData:
					newData.barName = spellData.Name;
					newData.barDefaultCooldown = spellData.spellModifierSettings.cooldown;
					newData.castTime = spellData.spellModifierSettings.castTime;
					break;
				case WeaponData weaponData:
					newData.barName = weaponData.Name;
					newData.barDefaultCooldown = weaponData.weaponModifierSettings.cooldown;
					newData.castTime = weaponData.weaponModifierSettings.castTime;
					break;
			}
			barCooldownData.Add(newData);
		}
	}
	private void Start()
	{
		for (int i = 0; i < spellBook.barItems.Count; i++)
		{
			if (spellBook.barItems[i] == null)
				continue;

			var newData = new BarCooldownData();
			Sprite icon = null;
			switch (spellBook.barItems[i])
			{
				case SpellData:
					if (spellBook.barItems[i] is SpellData spellData)
						icon = spellData.Icon;
					break;
				case WeaponData:
					if (spellBook.barItems[i] is WeaponData weaponData)
						icon = weaponData.Icon;
					break;
			}
			buttons[i].GetComponent<BarItemManager>().iconImg.sprite = icon;
			barCooldownData.Add(newData);
		}
	}

	private void BarSpellPressed(int index)
	{
		m_BarItemData = spellBook.barItems[index];

		if (m_BarItemData != null)
		{
			if (barCooldownData[index].barCurrentCooldown > 0)
			{
				Debug.Log("I can't do that yet");
				return;
			}
			if (isCasting == true)
			{
				Debug.Log("I am busy");
				return;
			}
			
			bool isCastSuccessful = InputEventManager.InteractionHandler(m_BarItemData);

			if (isCastSuccessful == true)
			{
				barCooldownData[index].barCurrentCooldown = barCooldownData[index].barDefaultCooldown;
				isCasting = true;
				StartCoroutine(CastingFinished(barCooldownData[index].castTime));
			}
		}
	}

	private void Update()
	{
		for (int i = 0; i < barCooldownData.Count; i++)
		{
			if (barCooldownData[i].barCurrentCooldown <= 0)
			{
				barCooldownData[i].barCurrentCooldown = 0;
				barCooldownData[i].isReady = true;
			}
			else
			{
				barCooldownData[i].barCurrentCooldown -= Time.deltaTime;
				barCooldownData[i].isReady = false;
			}
		}
		for (int i = 0; i < spellBook.barItems.Count; i++) //set cooldown image
		{
			if (spellBook.barItems[i] != null)
			{
				float fillAmount = barCooldownData[i].barCurrentCooldown / barCooldownData[i].barDefaultCooldown;
				buttons[i].GetComponent<BarItemManager>().cooldownImg.DOFillAmount(fillAmount, 0.01f);
			}
			else
			{
				buttons[i].GetComponent<BarItemManager>().cooldownImg.DOFillAmount(0, 0.01f);
			}
		}
	}
	private void SetIsCasting(float value)
	{
		isCasting = true;
		StartCoroutine(CastingFinished(value));
	}
	private IEnumerator CastingFinished(float value)
	{
		yield return new WaitForSeconds(value);
		isCasting = false;
	}

	private bool IsCastingContinue()
	{
		return isCasting;
	}
	

	// [Button]
	// private void OnValidate()
	// {
	// 	if (buttons.Length == 0)
	// 		return;
	//
	// 	for (int i = 0; i < slotsData.barItems.Length; i++)
	// 	{
	// 		if (slotsData.barItems[i] != null)
	// 			buttons[i].GetComponent<BarItemManager>().iconImg.sprite = slotsData.barItems[i].icon;
	// 	}
	// }
}
